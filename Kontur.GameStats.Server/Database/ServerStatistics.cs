using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kontur.GameStats.Server.Database
{
    public class ServerStatistics
    {
        [Key]
        public string Endpoint { get; set; }
        public int TotalMatchesPlayed { get; set; }
        public int MaximumMatchesPerDay { get; set; }
        public double AverageMatchesPerDay { get; set; }
        public int MaximumPopulation { get; set; }
        public double AveragePopulation { get; set; }
        public virtual List<StringEntry> Top5GameModes { get; set; }
        public virtual List<StringEntry> Top5Maps { get; set; }

        public virtual List<DayCountEntry> MatchesPerDay { get; set; }
        public virtual List<MatchCountEntry> PopulationPerMatch { get; set; }
        public virtual List<NameCountEntry> GameModePopularity { get; set; }
        public virtual List<NameCountEntry> MapPopularity { get; set; }
    }
}
