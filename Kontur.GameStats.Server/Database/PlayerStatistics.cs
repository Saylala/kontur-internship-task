using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kontur.GameStats.Server.Database
{
    public class PlayerStatistics
    {
        [Key]
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

        public virtual List<NameCountEntry> ServersPopularity { get; set; }
        public virtual List<NameCountEntry> GameModePopularity { get; set; }
        public virtual List<DayCountEntry> MatchesPerDay { get; set; }
        public int TotalKills { get; set; }
        public int TotalDeaths { get; set; }
    }
}
