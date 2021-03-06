﻿using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Kontur.GameStats.Server.Exceptions;
using Kontur.GameStats.Server.Routing;
using log4net;
using log4net.Config;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Kontur.GameStats.Server.Core
{
    public class StatServer : IDisposable
    {
        private readonly ILog logger = LogManager.GetLogger(typeof(StatServer));

        private readonly HttpListener listener;

        private Thread listenerThread;
        private bool disposed;
        private volatile bool isRunning;
        private readonly RouteHandler<Controller> routeHandler;

        public StatServer()
        {
            XmlConfigurator.Configure();
            AppDomain.CurrentDomain.SetData("DataDirectory", Directory.GetCurrentDirectory());
            JsonConvert.DefaultSettings =
                () => new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()};

            routeHandler = RouteHandler.Create(new Controller());
            listener = new HttpListener();
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
                result = await GetResponse(listenerContext);
                listenerContext.Response.StatusCode = (int) HttpStatusCode.OK;
            }
            catch (NotFoundException error)
            {
                logger.Error(error);
                listenerContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
            }
            catch (Exception error)
            {
                logger.Error(error);
                listenerContext.Response.StatusCode = (int) HttpStatusCode.BadRequest;
            }

            using (
                var writer = new StreamWriter(listenerContext.Response.OutputStream,
                    listenerContext.Request.ContentEncoding))
                writer.WriteLine(result);
        }

        private async Task<string> GetResponse(HttpListenerContext listenerContext)
        {
            var route = listenerContext.Request.Url.AbsolutePath;
            switch (listenerContext.Request.HttpMethod)
            {
                case "GET":
                    return await routeHandler.GetAsync(route);
                case "PUT":
                    var data =
                        new StreamReader(listenerContext.Request.InputStream, listenerContext.Request.ContentEncoding)
                            .ReadToEnd();
                    await routeHandler.PutAsync(route, data);
                    return string.Empty;
                default:
                    throw new InvalidRequestException($"Unsupported method : {listenerContext.Request.HttpMethod}");
            }
        }
    }
}