using System.Collections.Generic;
using System.Linq;
using Kontur.GameStats.Server.Models.DatabaseEntries;

namespace Kontur.GameStats.Server.Models.Serialization
{
    public class ServerStatistics
    {
        public ServerStatistics()
        {
        }

        public ServerStatistics(ServerStatisticsEntry statisticsEntry)
        {
            TotalMatchesPlayed = statisticsEntry.TotalMatchesPlayed;
            MaximumMatchesPerDay = statisticsEntry.MaximumMatchesPerDay;
            AverageMatchesPerDay = statisticsEntry.AverageMatchesPerDay;
            MaximumPopulation = statisticsEntry.MaximumPopulation;
            AveragePopulation = statisticsEntry.AveragePopulation;
            Top5GameModes = statisticsEntry.Top5GameModes.Select(x => x.String).ToList();
            Top5Maps = statisticsEntry.Top5Maps.Select(x => x.String).ToList();
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
