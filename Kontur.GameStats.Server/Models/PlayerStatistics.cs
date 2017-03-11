using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Kontur.GameStats.Server.Models
{
    [Table("PlayersStatistics")]
    public class PlayerStatistics
    {
        [Key]
        [JsonIgnore]
        public string Name { get; set; }

        public int TotalMatchesPlayed { get; set; }
        public int TotalMatchesWon { get; set; }
        public string FavoriteServer { get; set; }
        public int UniqueServers { get; set; }
        public string FavoriteGameMode { get; set; }
        public double AverageScoreboardPercent { get; set; }
        public int MaximumMatchesPerDay { get; set; }
        public double AverageMatchesPerDay { get; set; }
        public DateTime LastMatchPlayed { get; set; }
        public double KillToDeathRatio { get; set; }

        [JsonIgnore]
        public virtual List<NameCountEntry> ServersPopularity { get; set; }

        [JsonIgnore]
        public virtual List<NameCountEntry> GameModePopularity { get; set; }

        [JsonIgnore]
        public virtual List<DayCountEntry> MatchesPerDay { get; set; }

        [JsonIgnore]
        public int TotalKills { get; set; }

        [JsonIgnore]
        public int TotalDeaths { get; set; }
    }
}
