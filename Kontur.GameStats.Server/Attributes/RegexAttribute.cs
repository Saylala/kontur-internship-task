using System;
using System.Text.RegularExpressions;

namespace Kontur.GameStats.Server.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RegexAttribute : Attribute
    {
        public string Route { get; private set; }
        public Regex Regex { get; private set; }
        public RegexAttribute(string route)
        {
            Route = $"^{route}$";
            Regex = new Regex(route);
        }
    }
}
