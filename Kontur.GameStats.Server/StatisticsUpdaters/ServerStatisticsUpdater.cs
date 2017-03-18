using System;
using System.Collections.Generic;
using System.Linq;
using Kontur.GameStats.Server.Database;
using Kontur.GameStats.Server.Extentions;
using Kontur.GameStats.Server.Models.DatabaseEntries;

namespace Kontur.GameStats.Server.StatisticsUpdaters
{
    public class ServerStatisticsUpdater : IStatisticsUpdater
    {
        public void Update(MatchInfoEntry infoEntry, DatabaseContext databaseContext)
        {
            var previous = databaseContext.ServerStatistics.Find(infoEntry.Endpoint);
            if (previous == null)
                SetFirstEntry(infoEntry, databaseContext);
            else
                UpdateEntry(previous, infoEntry);

        }

        private void SetFirstEntry(MatchInfoEntry infoEntry, DatabaseContext databaseContext)
        {
            databaseContext.ServerStatistics.Add(new ServerStatisticsEntry
            {
                Endpoint = infoEntry.Endpoint,
                TotalMatchesPlayed = 1,
                MaximumMatchesPerDay = 1,
                AverageMatchesPerDay = 1,
                MaximumPopulation = infoEntry.Scoreboard.Count,
                AveragePopulation = infoEntry.Scoreboard.Count,
                Top5GameModes = new List<StringEntry> {new StringEntry {String = infoEntry.GameMode}},
                Top5Maps = new List<StringEntry> {new StringEntry {String = infoEntry.Map}},
                MatchesPerDay = new List<DayCountEntry> {new DayCountEntry {Day = infoEntry.Timestamp.Date, Count = 1}},
                PopulationPerMatch =
                    new List<MatchCountEntry>
                    {
                        new MatchCountEntry
                        {
                            Key = infoEntry.Endpoint + infoEntry.Timestamp,
                            Endpoint = infoEntry.Endpoint,
                            TimeStamp = infoEntry.Timestamp,
                            Count = infoEntry.Scoreboard.Count
                        }
                    },
                GameModePopularity =
                    new List<NameCountEntry> {new NameCountEntry {Name = infoEntry.GameMode, Count = 1}},
                MapPopularity = new List<NameCountEntry>
                {
                    new NameCountEntry {Name = infoEntry.Map, Count = 1}

                }
            });
        }

        private void UpdateEntry(ServerStatisticsEntry previous, MatchInfoEntry infoEntry)
        {
            previous.MatchesPerDay.AddOrUpdate(x => x.Day == infoEntry.Timestamp.Date,
                () => new DayCountEntry {Day = infoEntry.Timestamp.Date, Count = 1}, x => x.Count++);
            previous.PopulationPerMatch.AddOrUpdate(x => x.Key == infoEntry.Endpoint + infoEntry.Timestamp,
                () => new MatchCountEntry
                {
                    Key = infoEntry.Endpoint + infoEntry.Timestamp,
                    Endpoint = infoEntry.Endpoint,
                    TimeStamp = infoEntry.Timestamp,
                    Count = infoEntry.Scoreboard.Count
                }, x => x.Count++);
            previous.GameModePopularity.AddOrUpdate(x => x.Name == infoEntry.GameMode,
                () => new NameCountEntry {Name = infoEntry.GameMode, Count = 1}, x => x.Count++);
            previous.MapPopularity.AddOrUpdate(x => x.Name == infoEntry.Map,
                () => new NameCountEntry {Name = infoEntry.Map, Count = 1}, x => x.Count++);

            var totalDays = (previous.MatchesPerDay.Max(x => x.Day) - previous.MatchesPerDay.Min(x => x.Day)).TotalDays;
            previous.TotalMatchesPlayed = previous.TotalMatchesPlayed + 1;
            previous.MaximumMatchesPerDay = previous.MatchesPerDay.Select(x => x.Count).Max();
            previous.AverageMatchesPerDay = Math.Abs(totalDays) < 0.00001 ? previous.MatchesPerDay.Select(x => x.Count).Sum() : previous.MatchesPerDay.Select(x => x.Count).Sum() / totalDays;
            previous.MaximumPopulation = previous.PopulationPerMatch.Select(x => x.Count).Max();
            previous.AveragePopulation = (double) previous.PopulationPerMatch.Select(x => x.Count).Sum() / previous.PopulationPerMatch.Count;

            previous.Top5GameModes = previous.GameModePopularity
                .OrderByDescending(x => x.Count)
                .Take(5)
                .Select(x => new StringEntry {String = x.Name})
                .ToList();
            previous.Top5Maps = previous.MapPopularity
                .OrderByDescending(x => x.Count)
                .Take(5)
                .Select(x => new StringEntry {String = x.Name})
                .ToList();
        }
    }
}
