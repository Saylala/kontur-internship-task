using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Kontur.GameStats.Server.Models
{
    [Table("ServersStatistics")]
    public class ServerStatistics
    {
        [Key]
        [JsonIgnore]
        public string Endpoint { get; set; }

        public int TotalMatchesPlayed { get; set; }
        public int MaximumMatchesPerDay { get; set; }
        public double AverageMatchesPerDay { get; set; }
        public int MaximumPopulation { get; set; }
        public double AveragePopulation { get; set; }

        public virtual List<StringEntry> Top5GameModes { get; set; }
        public virtual List<StringEntry> Top5Maps { get; set; }

        [JsonIgnore]
        public virtual List<DayCountEntry> MatchesPerDay { get; set; }
        [JsonIgnore]
        public virtual List<MatchCountEntry> PopulationPerMatch { get; set; }
        [JsonIgnore]
        public virtual List<NameCountEntry> GameModePopularity { get; set; }
        [JsonIgnore]
        public virtual List<NameCountEntry> MapPopularity { get; set; }
    }
}
