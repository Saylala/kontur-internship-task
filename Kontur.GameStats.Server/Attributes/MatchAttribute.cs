using System;

namespace Kontur.GameStats.Server.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class MatchAttribute : Attribute
    {
        public string Route { get; private set; }
        // Attribute usage
        public MatchAttribute(string route)
        {
            Route = route;
        }
    }
}
