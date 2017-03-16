using System;
using System.Collections.Generic;
using System.Linq;
using Kontur.GameStats.Server.Database;
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
            var scoreboardPercent = (double)(infoEntry.Scoreboard.Count - position - 1) /
                                    (infoEntry.Scoreboard.Count - 1) * 100;
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
            var scoreboardPercent = (double)(infoEntry.Scoreboard.Count - position - 1) /
                                    (infoEntry.Scoreboard.Count - 1) * 100;
            var totalMatchesWon = position == 0
                ? previous.TotalMatchesWon + 1
                : previous.TotalMatchesWon;

            UpdateList(previous.ServersPopularity, infoEntry.Endpoint);
            UpdateList(previous.GameModePopularity, infoEntry.GameMode);
            UpdateList(previous.MatchesPerDay, infoEntry.Timestamp.Date);

            previous.TotalKills += infoEntry.Scoreboard[position].Kills;
            previous.TotalDeaths += infoEntry.Scoreboard[position].Deaths;

            var averageScoreboardPercent = (previous.AverageScoreboardPercent *
                                            previous.TotalMatchesPlayed + scoreboardPercent) /
                                           (previous.TotalMatchesPlayed + 1);
            var lastMatchPlayed = infoEntry.Timestamp > previous.LastMatchPlayed
                ? infoEntry.Timestamp
                : previous.LastMatchPlayed;
            var ignore = previous.TotalMatchesPlayed < 10 || previous.TotalDeaths == 0;
            var killToDeathRatio = ignore ? 0 : (double)previous.TotalKills / previous.TotalDeaths;
            previous.TotalMatchesPlayed = previous.TotalMatchesPlayed + 1;
            previous.TotalMatchesWon = totalMatchesWon;
            previous.FavoriteServer = previous.ServersPopularity.OrderByDescending(x => x.Count).First().Name;
            previous.UniqueServers = previous.ServersPopularity.Count;
            previous.FavoriteGameMode = previous.GameModePopularity.OrderByDescending(x => x.Count).First().Name;
            previous.AverageScoreboardPercent = averageScoreboardPercent;
            previous.MaximumMatchesPerDay = previous.MatchesPerDay.Select(x => x.Count).Max();
            previous.AverageMatchesPerDay = (double)previous.MatchesPerDay.Select(x => x.Count).Sum() / previous.MatchesPerDay.Count;
            previous.LastMatchPlayed = lastMatchPlayed;
            previous.KillToDeathRatio = killToDeathRatio;

        }

        private void UpdateList(List<NameCountEntry> list, string key)
        {
            var index = list.FindIndex(x => x.Name == key);
            if (index == -1)
                list.Add(new NameCountEntry { Name = key, Count = 1 });
            else
                list[index].Count++;
        }

        private void UpdateList(List<DayCountEntry> list, DateTime key)
        {
            var index = list.FindIndex(x => x.Day == key);
            if (index == -1)
                list.Add(new DayCountEntry { Day = key, Count = 1 });
            else
                list[index].Count++;
        }
    }
}
