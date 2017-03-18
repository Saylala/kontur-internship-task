using System.Collections.Generic;
using Kontur.GameStats.Server.Database;
using Kontur.GameStats.Server.Models.DatabaseEntries;

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

        public void Update(MatchInfoEntry infoEntry, DatabaseContext databaseContext)
        {
            foreach (var statisticsUpdater in updaters)
                statisticsUpdater.Update(infoEntry, databaseContext);
        }
    }
}
