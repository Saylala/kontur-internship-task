using System;
using Kontur.GameStats.Server.Models.DatabaseEntries;

namespace Kontur.GameStats.Server.Models.Serialization
{
    public class PlayerStatistics
    {
        public PlayerStatistics()
        {
        }

        public PlayerStatistics(PlayerStatisticsEntry entry)
        {
            TotalMatchesPlayed = entry.TotalMatchesPlayed;
            TotalMatchesWon = entry.TotalMatchesWon;
            FavoriteServer = entry.FavoriteServer;
            UniqueServers = entry.UniqueServers;
            FavoriteGameMode = entry.FavoriteGameMode;
            AverageScoreboardPercent = entry.AverageScoreboardPercent;
            MaximumMatchesPerDay = entry.MaximumMatchesPerDay;
            AverageMatchesPerDay = entry.AverageMatchesPerDay;

            LastMatchPlayed = DateTime.SpecifyKind(entry.LastMatchPlayed, DateTimeKind.Utc);

            KillToDeathRatio = entry.KillToDeathRatio;
        }

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
    }
}
