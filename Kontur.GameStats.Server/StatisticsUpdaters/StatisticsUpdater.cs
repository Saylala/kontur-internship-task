using System.Collections.Generic;
using Kontur.GameStats.Server.Models;

namespace Kontur.GameStats.Server.StatisticsUpdaters
{
    public class StatisticsUpdater
    {
        private readonly List<IStatisticsUpdater> updaters;
        public StatisticsUpdater()
        {
            updaters = new List<IStatisticsUpdater>
            {
                new ServerStatisticsUpdater(),
                new PlayerStatisticsUpdater(),
                new RecentMatchesUpdater(),
                new BestPlayersUpdater(),
                new PopularServersUpdater(),
            };
        }

        public void Update(MatchInfo info)
        {
            foreach (var statisticsUpdater in updaters)
                statisticsUpdater.Update(info);
        }
    }
}
