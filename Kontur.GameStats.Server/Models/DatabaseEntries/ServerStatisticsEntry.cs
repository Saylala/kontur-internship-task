using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kontur.GameStats.Server.Models.DatabaseEntries
{
    [Table("ServersStatistics")]
    public class ServerStatisticsEntry
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
