using System;
using System.Collections.Generic;
using Kontur.GameStats.Server.Database;
using Kontur.GameStats.Server.Models;

namespace Kontur.GameStats.Server.StatisticsUpdaters
{
    public class StatisticsUpdater
    {
        private readonly List<IStatisticsUpdater> updaters;

        public StatisticsUpdater(SortedList<DateTime, RecentMatch> recentMatches,
            SortedList<double, BestPlayer> bestPlayers, SortedList<double, PopularServer> popularServers)
        {
            updaters = new List<IStatisticsUpdater>
            {
                new ServerStatisticsUpdater(),
                new PlayerStatisticsUpdater(),
                new RecentMatchesUpdater(recentMatches),
                new BestPlayersUpdater(bestPlayers),
                new PopularServersUpdater(popularServers)
            };
        }

        public void Update(MatchInfo info, DatabaseContext databaseContext)
        {
            foreach (var statisticsUpdater in updaters)
                statisticsUpdater.Update(info, databaseContext);
        }
    }
}
