using System;
using System.Collections.Generic;
using Kontur.GameStats.Server.Database;
using Kontur.GameStats.Server.Models;

namespace Kontur.GameStats.Server.StatisticsUpdaters
{
    public class RecentMatchesUpdater : IStatisticsUpdater
    {
        private readonly SortedList<DateTime, RecentMatch> recentMatches;
        private const int maxServersCount = 50;

        public RecentMatchesUpdater(SortedList<DateTime, RecentMatch> recentMatches)
        {
            this.recentMatches = recentMatches;
        }

        public void Update(MatchInfo info, DatabaseContext databaseContext)
        {
            if (recentMatches.Count < maxServersCount)
                recentMatches.Add(
                    info.Timestamp,
                    new RecentMatch
                    {
                        Key = info.Key,
                        Server = info.Endpoint,
                        Timestamp = info.Timestamp,
                    });
            else if (recentMatches.Values[recentMatches.Count - 1].Timestamp < info.Timestamp)
            {
                recentMatches.RemoveAt(recentMatches.Count - 1);
                recentMatches.Add(
                    info.Timestamp,
                    new RecentMatch
                    {
                        Key = info.Key,
                        Server = info.Endpoint,
                        Timestamp = info.Timestamp,
                    });
            }
        }
    }
}
