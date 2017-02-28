using System;
using System.Collections.Generic;
using System.Linq;
using Kontur.GameStats.Server.Database;
using Kontur.GameStats.Server.Models;

namespace Kontur.GameStats.Server.StatisticsUpdaters
{
    public class PlayerStatisticsUpdater : IStatisticsUpdater
    {
        public void Update(MatchInfo info)
        {
            using (var databaseContext = new DatabaseContext())
            {
                foreach (var player in info.Scoreboard)
                {
                    var previous = databaseContext.PlayersStatistics.Find(player.Name);
                    if (previous == null)
                        SetFirstEntry(info, player.Name, databaseContext);
                    else
                        UpdateEntry(previous, info);
                }

                databaseContext.SaveChanges();
            }
        }

        private void SetFirstEntry(MatchInfo info, string name, DatabaseContext databaseContext)
        {
            var position = info.Scoreboard.FindIndex(x => x.Name == name);
            var scoreboardPercent = (double)(info.Scoreboard.Count - position - 1) /
                                    (info.Scoreboard.Count - 1) * 100;
            databaseContext.PlayersStatistics.Add(new PlayerStatistics
            {
                Name = name,
                TotalMatchesPlayed = 1,
                TotalMatchesWon = position == 0 ? 1 : 0,
                FavoriteServer = info.Endpoint,
                UniqueServers = 1,
                FavoriteGameMode = info.GameMode,
                AverageScoreboardPercent = scoreboardPercent,
                MaximumMatchesPerDay = 1,
                AverageMatchesPerDay = 1,
                LastMatchPlayed = info.Timestamp,
                KillToDeathRatio = 0,
                ServersPopularity = new List<NameCountEntry> { new NameCountEntry { Name = info.Endpoint, Count = 1 } },
                GameModePopularity = new List<NameCountEntry> { new NameCountEntry { Name = info.GameMode, Count = 1 } },
                MatchesPerDay = new List<DayCountEntry> { new DayCountEntry { Day = info.Timestamp.Date, Count = 1 } },
                TotalKills = info.Scoreboard[position].Kills,
                TotalDeaths = info.Scoreboard[position].Deaths
            });
        }

        private void UpdateEntry(PlayerStatistics previous, MatchInfo info)
        {
            var position = info.Scoreboard.FindIndex(x => x.Name == previous.Name);
            var scoreboardPercent = (double)(info.Scoreboard.Count - position - 1) /
                                    (info.Scoreboard.Count - 1) * 100;
            var totalMatchesWon = position == 0
                ? previous.TotalMatchesWon + 1
                : previous.TotalMatchesWon;
            UpdateList(previous.ServersPopularity, info.Endpoint);
            UpdateList(previous.GameModePopularity, info.GameMode);
            UpdateList(previous.MatchesPerDay, info.Timestamp.Date);
            previous.TotalKills += info.Scoreboard[position].Kills;
            previous.TotalDeaths += info.Scoreboard[position].Deaths;

            var averageScoreboardPercent = (previous.AverageScoreboardPercent *
                                            previous.TotalMatchesPlayed + scoreboardPercent) /
                                           (previous.TotalMatchesPlayed + 1);
            var lastMatchPlayed = info.Timestamp > previous.LastMatchPlayed
                ? info.Timestamp
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
