using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
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

        public RouteHandler(TController controller)
        {
            this.controller = controller;

            putMethods = GetMethods(typeof(TController), isPut: true);
            getMethods = GetMethods(typeof(TController), isPut: false);
        }

        public void Put(string route, string json)
        {
            var regex = putMethods.Keys.FirstOrDefault(x => x.IsMatch(route));
            if (regex == null)
                throw new InvalidRequestException("Invalid Request");

            var methodInfo = putMethods[regex];
            var groups = regex.Match(route).Groups;
            var parameters = putMethods[regex].GetParameters();
            var arguments = parameters
                .Skip(1)
                .Select(x => GetParameter(x, groups[x.Name].Value));

            var parameterType = parameters.First().ParameterType;
            var argumentsList = new List<object> {JsonConvert.DeserializeObject(json, parameterType)};
            argumentsList.AddRange(arguments);

            methodInfo.Invoke(controller, argumentsList.ToArray());
        }

        public string Get(string route)
        {
            var regex = getMethods.Keys.FirstOrDefault(x => x.IsMatch(route));
            if (regex == null)
                throw new InvalidRequestException("Invalid Request");

            var methodInfo = getMethods[regex];
            var groups = regex.Match(route).Groups;
            var arguments = getMethods[regex].GetParameters()
                .Select(x => GetParameter(x, groups[x.Name].Value))
                .ToArray();

            return JsonConvert.SerializeObject(methodInfo.Invoke(controller, arguments));
        }

        private static object GetParameter(ParameterInfo parameterInfo, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                if (parameterInfo.HasDefaultValue)
                    return Type.Missing;

                if (parameterInfo.IsOptional)
                    return null;
            }

            try
            {
                return JsonConvert.DeserializeObject($"\"{value}\"", parameterInfo.ParameterType);
            }
            catch (JsonReaderException e)
            {
                throw new InvalidRequestException(e.Message);
            }
        }

        private static Dictionary<Regex, MethodInfo> GetMethods(Type type, bool isPut)
        {
            return type
                .GetMethods()
                .Where(x => x.GetCustomAttributes<RouteAttribute>().Any() &&
                            x.GetCustomAttributes<PutAttribute>().Any() == isPut)
                .ToDictionary(x => x.GetCustomAttributes<RouteAttribute>().Single().Regex, x => x);
        }
    }
}
