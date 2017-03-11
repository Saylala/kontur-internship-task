using System.Collections.Generic;
using System.Linq;
using Kontur.GameStats.Server.Database;
using Kontur.GameStats.Server.Models;

namespace Kontur.GameStats.Server.StatisticsUpdaters
{
    public class PopularServersUpdater : IStatisticsUpdater
    {
        private readonly SortedList<double, PopularServer> popularServers;
        private const int maxServersCount = 50;

        public PopularServersUpdater(SortedList<double, PopularServer> popularServers)
        {
            this.popularServers = popularServers;
        }

        public void Update(MatchInfo info, DatabaseContext databaseContext)
        {
            var serverInfo = databaseContext.ServerStatistics.Find(info.Endpoint);

            if (serverInfo == null)
            {
                return;
            }

            var serverName = databaseContext.Servers.Find(info.Endpoint).Name;


            //var previous = popularServers.Find(x => x.Name == serverName);

            //if (previous != null)
            //    previous.AverageMatchesPerDay = serverInfo.AverageMatchesPerDay;


            if (popularServers.Count < maxServersCount)
                popularServers.Add(
                    serverInfo.AverageMatchesPerDay,
                    new PopularServer
                    {
                        Endpoint = info.Endpoint,
                        Name = serverName,
                        AverageMatchesPerDay = serverInfo.AverageMatchesPerDay
                    });
            else if (popularServers[popularServers.Count - 1].AverageMatchesPerDay < serverInfo.AverageMatchesPerDay)
            {
                popularServers.RemoveAt(popularServers.Count - 1);
                popularServers.Add(
                    serverInfo.AverageMatchesPerDay,
                    new PopularServer
                    {
                        Endpoint = info.Endpoint,
                        Name = serverName,
                        AverageMatchesPerDay = serverInfo.AverageMatchesPerDay
                    });
            }
        }
    }
}
