namespace Kontur.GameStats.Server.Database
{
    public class StatisticsUpdater
    {
        public StatisticsUpdater()
        {
            serverStatisticsUpdater = new ServerStatisticsUpdater();
            playerStatisticsUpdater = new PlayerStatisticsUpdater();
            recentMatchesUpdater = new RecentMatchesUpdater();
            bestPlayersUpdater = new BestPlayersUpdater();
            popularServersUpdater =new PopularServersUpdater();
        }

        private readonly ServerStatisticsUpdater serverStatisticsUpdater;
        private readonly PlayerStatisticsUpdater playerStatisticsUpdater;
        private readonly RecentMatchesUpdater recentMatchesUpdater;
        private readonly BestPlayersUpdater bestPlayersUpdater;
        private readonly PopularServersUpdater popularServersUpdater;

        public void Update(MatchInfo info, DatabaseContext databaseContext)
        {
            serverStatisticsUpdater.Update(info, databaseContext);
            //playerStatisticsUpdater.Update(info, databaseContext);
            //recentMatchesUpdater.Update(info, databaseContext);
            //bestPlayersUpdater.Update(info, databaseContext);
            //popularServersUpdater.Update(info, database);
        }
    }
}
