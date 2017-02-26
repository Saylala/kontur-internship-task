using System;
using System.Collections.Generic;
using System.Linq;

namespace Kontur.GameStats.Server.Database
{
    public class ServerStatisticsUpdater
    {
        public void Update(MatchInfo info, DatabaseContext databaseContext)
        {
            var previous = databaseContext.ServerStatistics.Find(info.Endpoint);
            if (previous == null)
                SetFirstEntry(info, databaseContext);
            else
                UpdateEntry(previous, info);
        }

        private void SetFirstEntry(MatchInfo info, DatabaseContext databaseContext)
        {
            databaseContext.ServerStatistics.Add(new ServerStatistics
            {
                Endpoint = info.Endpoint,
                TotalMatchesPlayed = 1,
                MaximumMatchesPerDay = 1,
                AverageMatchesPerDay = 1,
                MaximumPopulation = info.Scoreboard.Count,
                AveragePopulation = info.Scoreboard.Count,
                Top5GameModes = new List<StringEntry> { new StringEntry { String = info.GameMode } },
                Top5Maps = new List<StringEntry> { new StringEntry { String = info.Map } },
                MatchesPerDay = new List<DayCountEntry> { new DayCountEntry { Day = info.Timestamp.Date, Count = 1 } },
                PopulationPerMatch =
                    new List<MatchCountEntry>
                    {
                        new MatchCountEntry { Key = info.Endpoint + info.Timestamp, Endpoint =  info.Endpoint, TimeStamp = info.Timestamp, Count = info.Scoreboard.Count}
                    },
                GameModePopularity = new List<NameCountEntry> { new NameCountEntry { Name = info.GameMode, Count = 1 } },
                MapPopularity = new List<NameCountEntry> { new NameCountEntry {Name = info.Map, Count = 1 } }
            });
        }

        private void UpdateEntry(ServerStatistics previous, MatchInfo info)
        {
            UpdateList(previous.MatchesPerDay, info.Timestamp.Date);
            UpdateList(previous.PopulationPerMatch, info.Endpoint, info.Timestamp);
            UpdateList(previous.GameModePopularity, info.GameMode);
            UpdateList(previous.MapPopularity, info.Map);
            previous.TotalMatchesPlayed = previous.TotalMatchesPlayed + 1;
            previous.MaximumMatchesPerDay = previous.MatchesPerDay.Select(x => x.Count).Max();
            previous.AverageMatchesPerDay = (double)previous.MatchesPerDay.Select(x => x.Count).Sum() / previous.MatchesPerDay.Count;
            previous.MaximumPopulation = previous.PopulationPerMatch.Select(x => x.Count).Max();
            previous.AveragePopulation = (double)previous.PopulationPerMatch.Select(x => x.Count).Sum() / previous.PopulationPerMatch.Count;
            previous.Top5GameModes = previous.GameModePopularity
                .OrderByDescending(x => x.Count)
                .Take(5)
                .Select(x => new StringEntry { String = x.Name })
                .ToList();
            previous.Top5Maps = previous.MapPopularity
                .OrderByDescending(x => x.Count)
                .Take(5)
                .Select(x => new StringEntry { String = x.Name })
                .ToList();
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

        private void UpdateList(List<MatchCountEntry> list, string endpoint, DateTime timeStamp)
        {
            var key = endpoint + timeStamp;
            var index = list.FindIndex(x => x.Key == key);
            if (index == -1)
                list.Add(new MatchCountEntry { Key = key, Endpoint = endpoint, TimeStamp = timeStamp, Count = 1 });
            else
                list[index].Count++;
        }
    }
}
