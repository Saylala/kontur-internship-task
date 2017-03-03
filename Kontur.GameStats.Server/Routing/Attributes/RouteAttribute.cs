using System;
using System.Text.RegularExpressions;

namespace Kontur.GameStats.Server.Routing.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RouteAttribute : Attribute
    {
        public string Route { get; private set; }
        public Regex Regex { get; private set; }

        public RouteAttribute(string route)
        {
            Route = route;
            Regex = new Regex("^" + route.Replace("[", "(|")
                                  .Replace("]", ")")
                                  .Replace("<", "(?<")
                                  .Replace(">", ">.+?)") + "$");
        }
    }
}
