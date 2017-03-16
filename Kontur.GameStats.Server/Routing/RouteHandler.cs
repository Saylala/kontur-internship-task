using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Fclp.Internals.Extensions;
using Kontur.GameStats.Server.Exceptions;
using Kontur.GameStats.Server.Routing.Attributes;
using Newtonsoft.Json;

namespace Kontur.GameStats.Server.Routing
{
    public class RouteHandler
    {
        public static RouteHandler<TController> Create<TController>(TController controller)
        {
            return new RouteHandler<TController>(controller);
        }
    }

    public class RouteHandler<TController>
    {
        private readonly TController controller;
        private readonly Dictionary<Regex, MethodInfo> putMethods;
        private readonly Dictionary<Regex, MethodInfo> getMethods;
        private readonly Dictionary<MethodInfo, ParameterInfo[]> parametersCache;

        public RouteHandler(TController controller)
        {
            this.controller = controller;

            parametersCache = new Dictionary<MethodInfo, ParameterInfo[]>();
            putMethods = GetMethods(typeof(TController), isPut: true);
            getMethods = GetMethods(typeof(TController), isPut: false);
        }

        public void Put(string route, string json)
        {
            PutAsync(route, json).Wait();
        }

        public string Get(string route)
        {
            return GetAsync(route).Result;
        }

        private KeyValuePair<GroupCollection, MethodInfo> GetMatch(string route, Dictionary<Regex, MethodInfo> methods)
        {
            var regex = methods.Keys.FirstOrDefault(x => x.IsMatch(route));
            if (regex == null)
                throw new InvalidRequestException("Invalid Request");

            return new KeyValuePair<GroupCollection, MethodInfo>(regex.Match(route).Groups, methods[regex]);
        }

        private object[] GetArguments(KeyValuePair<GroupCollection, MethodInfo> match, int skipCount = 0)
        {
            return parametersCache[match.Value]
                .Skip(skipCount)
                .Select(x => GetParameter(x, match.Key[x.Name].Value))
                .ToArray();
        }

        public async Task PutAsync(string route, string json)
        {
            var match = GetMatch(route, putMethods);

            var jsonParameter = JsonConvert.DeserializeObject(json, parametersCache[match.Value].First().ParameterType);
            var argumentsList = new List<object> { jsonParameter };
            argumentsList.AddRange(GetArguments(match, 1));

            try
            {
                var result = match.Value.Invoke(controller, argumentsList.ToArray());
                var task = result as Task;
                if (task != null)
                    await task;
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException ?? e;
            }
            catch (AggregateException e)
            {
                throw e.InnerExceptions.FirstOrDefault() ?? e;
            }
        }

        public async Task<string> GetAsync(string route)
        {
            var match = GetMatch(route, getMethods);
            var arguments = GetArguments(match);

            try
            {
                var methodResult = match.Value.Invoke(controller, arguments);
                var taskResult = methodResult as Task;
                return taskResult != null
                    ? JsonConvert.SerializeObject(await (dynamic)methodResult)
                    : JsonConvert.SerializeObject(methodResult);
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException ?? e;
            }
            catch (AggregateException e)
            {
                throw e.InnerExceptions.FirstOrDefault() ?? e;
            }
        }

        private static object GetParameter(ParameterInfo parameterInfo, string value)
        {
            if (string.IsNullOrEmpty(value) && parameterInfo.HasDefaultValue)
                return Type.Missing;

            try
            {
                return JsonConvert.DeserializeObject($"\"{value}\"", parameterInfo.ParameterType);
            }
            catch (JsonReaderException e)
            {
                throw new InvalidRequestException(e.Message);
            }
        }

        private Dictionary<Regex, MethodInfo> GetMethods(Type type, bool isPut)
        {
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.GetCustomAttributes<RouteAttribute>().Any() &&
                            x.GetCustomAttributes<PutAttribute>().Any() == isPut)
                .ToDictionary(x => x.GetCustomAttributes<RouteAttribute>().Single().Regex, x => x);

            methods.Values.ForEach(x => parametersCache[x] = x.GetParameters());

            return methods;
        }
    }
}
