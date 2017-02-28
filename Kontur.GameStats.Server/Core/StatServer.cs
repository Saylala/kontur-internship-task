using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Kontur.GameStats.Server.Attributes;
using Kontur.GameStats.Server.Exceptions;
using log4net;
using Newtonsoft.Json;

namespace Kontur.GameStats.Server.Core
{
    public class StatServer : IDisposable
    {
        private readonly ILog logger = LogManager.GetLogger(typeof(StatServer));

        private readonly HttpListener listener;

        private Thread listenerThread;
        private bool disposed;
        private volatile bool isRunning;
        private readonly Controller controller;
        private readonly Dictionary<Regex, MethodInfo> putMethods;
        private readonly Dictionary<Regex, MethodInfo> getMethods;

        public StatServer()
        {
            controller = new Controller();
            listener = new HttpListener();
            putMethods = GetMethods(typeof(Controller), true);
            getMethods = GetMethods(typeof(Controller), false);
        }

        public void Start(string prefix)
        {
            lock (listener)
            {
                if (!isRunning)
                {
                    listener.Prefixes.Clear();
                    listener.Prefixes.Add(prefix);
                    listener.Start();

                    listenerThread = new Thread(Listen)
                    {
                        IsBackground = true,
                        Priority = ThreadPriority.Highest
                    };
                    listenerThread.Start();

                    isRunning = true;
                }
            }
        }

        public void Stop()
        {
            lock (listener)
            {
                if (!isRunning)
                    return;

                listener.Stop();

                listenerThread.Abort();
                listenerThread.Join();

                isRunning = false;
            }
        }

        public void Dispose()
        {
            if (disposed)
                return;

            disposed = true;

            Stop();

            listener.Close();
        }

        private void Listen()
        {
            while (true)
            {
                try
                {
                    if (listener.IsListening)
                    {
                        var context = listener.GetContext();
                        Task.Run(() => HandleContextAsync(context));
                    }
                    else Thread.Sleep(0);
                }
                catch (ThreadAbortException)
                {
                    return;
                }
                catch (Exception error)
                {
                    logger.Error(error);
                }
            }
        }

        private async Task HandleContextAsync(HttpListenerContext listenerContext)
        {
            logger.Info($"{listenerContext.Request.HttpMethod} {listenerContext.Request.Url.AbsolutePath}");
            var result = string.Empty;
            try
            {
                result = GetResponse(listenerContext);
                listenerContext.Response.StatusCode = (int) HttpStatusCode.OK;
            }
            catch (NotFoundException error)
            {
                logger.Error(error.ToString());
                listenerContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }
            catch (Exception error)
            {
                logger.Error(error.ToString());
                listenerContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }

            using (var writer = new StreamWriter(listenerContext.Response.OutputStream))
                writer.WriteLine(result);
        }

        private string GetResponse(HttpListenerContext listenerContext)
        {
            var route = listenerContext.Request.Url.AbsolutePath;

            var isGet = listenerContext.Request.HttpMethod == "GET";
            var dict = isGet ? getMethods : putMethods;

            var regex = dict.Keys.FirstOrDefault(x => x.IsMatch(route));
            if (regex == null)
                throw new InvalidRequestException("Invalid Request");
            var groups = regex.Match(route).Groups;

            var arguments = Enumerable.Range(1, groups.Count - 1).Select(x => (object)groups[x].Value).ToList();

            if (!isGet)
            {
                var json = new StreamReader(listenerContext.Request.InputStream).ReadToEnd();
                var parameterType = dict[regex].GetParameters().Last().ParameterType;
                arguments.Add(JsonConvert.DeserializeObject(json, parameterType));
            }

            return (string)dict[regex].Invoke(controller, arguments.ToArray());
        }

        private Dictionary<Regex, MethodInfo> GetMethods(Type type, bool isPut)
        {
            return type
                .GetMethods()
                .Where(x => x.GetCustomAttributes<RegexAttribute>().Any() && (x.GetCustomAttributes<PutAttribute>().Any() == isPut))
                .ToDictionary(x => x.GetCustomAttributes<RegexAttribute>().Single().Regex, x => x);
        }
    }
}