using System.Collections.Generic;
using System.Linq;

namespace Kontur.GameStats.Server.Models.Serialization
{
    public class ServerStatistics
    {
        public ServerStatistics() { }

        public ServerStatistics(Models.ServerStatistics statistics)
        {
            TotalMatchesPlayed = statistics.TotalMatchesPlayed;
            MaximumMatchesPerDay = statistics.MaximumMatchesPerDay;
            AverageMatchesPerDay = statistics.AverageMatchesPerDay;
            MaximumPopulation = statistics.MaximumPopulation;
            AveragePopulation = statistics.AveragePopulation;
            Top5GameModes = statistics.Top5GameModes.Select(x => x.String).ToList();
            Top5Maps = statistics.Top5Maps.Select(x => x.String).ToList();
        }

        public int TotalMatchesPlayed { get; set; }
        public int MaximumMatchesPerDay { get; set; }
        public double AverageMatchesPerDay { get; set; }
        public int MaximumPopulation { get; set; }
        public double AveragePopulation { get; set; }
        public List<string> Top5GameModes { get; set; }
        public List<string> Top5Maps { get; set; }
    }
}
