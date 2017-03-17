using System;
using System.Collections.Generic;
using System.Linq;
using Kontur.GameStats.Server.Database;
using Kontur.GameStats.Server.Extentions;
using Kontur.GameStats.Server.Models.DatabaseEntries;

namespace Kontur.GameStats.Server.StatisticsUpdaters
{
    public class PlayerStatisticsUpdater : IStatisticsUpdater
    {
        public void Update(MatchInfoEntry infoEntry, DatabaseContext databaseContext)
        {
            foreach (var player in infoEntry.Scoreboard)
            {
                var previous = databaseContext.PlayersStatistics.Find(player.Name);
                if (previous == null)
                    SetFirstEntry(infoEntry, player.Name, databaseContext);
                else
                    UpdateEntry(previous, infoEntry);
            }
        }

        private void SetFirstEntry(MatchInfoEntry infoEntry, string name, DatabaseContext databaseContext)
        {
            var position = infoEntry.Scoreboard.FindIndex(x => x.Name == name);
            var scoreboardPercent = 100.0;
            if (infoEntry.Scoreboard.Count > 1)
                scoreboardPercent = (double)(infoEntry.Scoreboard.Count - position - 1) / (infoEntry.Scoreboard.Count - 1) * 100;
            databaseContext.PlayersStatistics.Add(new PlayerStatisticsEntry
            {
                Name = name,
                TotalMatchesPlayed = 1,
                TotalMatchesWon = position == 0 ? 1 : 0,
                FavoriteServer = infoEntry.Endpoint,
                UniqueServers = 1,
                FavoriteGameMode = infoEntry.GameMode,
                AverageScoreboardPercent = scoreboardPercent,
                MaximumMatchesPerDay = 1,
                AverageMatchesPerDay = 1,
                LastMatchPlayed = infoEntry.Timestamp,
                KillToDeathRatio = 0,
                ServersPopularity = new List<NameCountEntry> { new NameCountEntry { Name = infoEntry.Endpoint, Count = 1 } },
                GameModePopularity = new List<NameCountEntry> { new NameCountEntry { Name = infoEntry.GameMode, Count = 1 } },
                MatchesPerDay = new List<DayCountEntry> { new DayCountEntry { Day = infoEntry.Timestamp.Date, Count = 1 } },
                TotalKills = infoEntry.Scoreboard[position].Kills,
                TotalDeaths = infoEntry.Scoreboard[position].Deaths
            });
        }

        private void UpdateEntry(PlayerStatisticsEntry previous, MatchInfoEntry infoEntry)
        {
            var position = infoEntry.Scoreboard.FindIndex(x => x.Name == previous.Name);

            var scoreboardPercent = 100.0;
            if (infoEntry.Scoreboard.Count > 1)
                scoreboardPercent = (double) (infoEntry.Scoreboard.Count - position - 1) / (infoEntry.Scoreboard.Count - 1) * 100;

            var totalMatchesWon = position == 0 ? previous.TotalMatchesWon + 1 : previous.TotalMatchesWon;

            previous.ServersPopularity.AddOrUpdate(x => x.Name == infoEntry.Endpoint,
                () => new NameCountEntry {Name = infoEntry.GameMode, Count = 1}, x => x.Count++);
            previous.GameModePopularity.AddOrUpdate(x => x.Name == infoEntry.GameMode,
                () => new NameCountEntry {Name = infoEntry.GameMode, Count = 1}, x => x.Count++);
            previous.MatchesPerDay.AddOrUpdate(x => x.Day == infoEntry.Timestamp.Date,
                () => new DayCountEntry {Day = infoEntry.Timestamp.Date, Count = 1}, x => x.Count++);

            previous.TotalKills += infoEntry.Scoreboard[position].Kills;
            previous.TotalDeaths += infoEntry.Scoreboard[position].Deaths;

            var totalDays = (previous.MatchesPerDay.Max(x => x.Day) - previous.MatchesPerDay.Min(x => x.Day)).TotalDays;
            var averageScoreboardPercent = (previous.AverageScoreboardPercent * previous.TotalMatchesPlayed + scoreboardPercent) / (previous.TotalMatchesPlayed + 1);
            var lastMatchPlayed = infoEntry.Timestamp > previous.LastMatchPlayed ? infoEntry.Timestamp : previous.LastMatchPlayed;
            var noDeaths = previous.TotalDeaths == 0;
            previous.TotalMatchesPlayed = previous.TotalMatchesPlayed + 1;
            previous.TotalMatchesWon = totalMatchesWon;
            previous.FavoriteServer = previous.ServersPopularity.OrderByDescending(x => x.Count).First().Name;
            previous.UniqueServers = previous.ServersPopularity.Count;
            previous.FavoriteGameMode = previous.GameModePopularity.OrderByDescending(x => x.Count).First().Name;
            previous.AverageScoreboardPercent = averageScoreboardPercent;
            previous.MaximumMatchesPerDay = previous.MatchesPerDay.Select(x => x.Count).Max();
            previous.AverageMatchesPerDay = Math.Abs(totalDays) < 0.01 ? previous.MatchesPerDay.Select(x => x.Count).Sum() : previous.MatchesPerDay.Select(x => x.Count).Sum() / totalDays;
            previous.LastMatchPlayed = lastMatchPlayed;
            previous.KillToDeathRatio = noDeaths ? previous.TotalKills : (double) previous.TotalKills / previous.TotalDeaths;
        }
    }
}
