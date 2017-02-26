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
using Newtonsoft.Json;

namespace Kontur.GameStats.Server
{
    internal class StatServer : IDisposable
    {
        public StatServer()
        {
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
                    // TODO: log errors
                    // log4net
                }
            }
        }

        private async Task HandleContextAsync(HttpListenerContext listenerContext)
        {
            string result;
            try
            {
                result = GetResponse(listenerContext);
            }
            catch (Exception error)
            {
                result = error.ToString();
            }

            listenerContext.Response.StatusCode = (int)HttpStatusCode.OK;
            using (var writer = new StreamWriter(listenerContext.Response.OutputStream))
                writer.WriteLine(result);
        }

        private string GetResponse(HttpListenerContext listenerContext)
        {
            var route = listenerContext.Request.Url.AbsolutePath;

            var isGet = listenerContext.Request.HttpMethod == "GET";
            var dict = isGet ? getMethods : putMethods;

            var matched = dict.Keys.First(x => new Regex(x).IsMatch(route));
            var groups = new Regex(matched).Match(route).Groups;

            var arguments = Enumerable.Range(1, groups.Count - 1).Select(x => (object)groups[x].Value).ToList();

            if (!isGet)
            {
                var json = new StreamReader(listenerContext.Request.InputStream).ReadToEnd();
                var parameterType = dict[matched].GetParameters().Last().ParameterType;
                arguments.Add(JsonConvert.DeserializeObject(json, parameterType));
            }

            //var test = (string)dict[matched].Invoke(controller, arguments.ToArray());
            return (string)dict[matched].Invoke(controller, arguments.ToArray());
        }

        private Dictionary<string, MethodInfo> GetMethods(Type type, bool isPut)
        {
            return type
                .GetMethods()
                .Where(x => x.GetCustomAttributes<MatchAttribute>().Any() && (x.GetCustomAttributes<PutAttribute>().Any() == isPut))
                .ToDictionary(x => x.GetCustomAttributes<MatchAttribute>().Single().Route, x => x);
        }

        private readonly HttpListener listener;

        private Thread listenerThread;
        private bool disposed;
        private volatile bool isRunning;
        private readonly Controller controller = new Controller();
        private readonly Dictionary<string, MethodInfo> putMethods;
        private readonly Dictionary<string, MethodInfo> getMethods;
    }
}