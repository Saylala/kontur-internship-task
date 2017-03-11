using System.Linq;
using Kontur.GameStats.Server.Database;
using Kontur.GameStats.Server.Models;

namespace Kontur.GameStats.Server.StatisticsUpdaters
{
    public class PopularServersUpdater : IStatisticsUpdater
    {
        public void Update(MatchInfo info, DatabaseContext databaseContext)
        {
            const int maxServersCount = 50;

            var popularServers = databaseContext.PopularServers.OrderByDescending(x => x.AverageMatchesPerDay).ToList();
            var serverInfo = databaseContext.ServerStatistics.Find(info.Endpoint);

            if (serverInfo == null)
            {
                return;
            }

            var serverName = databaseContext.Servers.Find(info.Endpoint).Name;
            var previous = popularServers.Find(x => x.Name == serverName);

            if (previous != null)
                previous.AverageMatchesPerDay = serverInfo.AverageMatchesPerDay;
            else if (popularServers.Count < maxServersCount)
                databaseContext.PopularServers.Add(new PopularServer
                {
                    Endpoint = info.Endpoint,
                    Name = serverName,
                    AverageMatchesPerDay = serverInfo.AverageMatchesPerDay
                });
            else if (popularServers[popularServers.Count - 1].AverageMatchesPerDay < serverInfo.AverageMatchesPerDay)
            {
                databaseContext.PopularServers.Remove(popularServers[popularServers.Count - 1]);
                databaseContext.PopularServers.Add(new PopularServer
                {
                    Endpoint = info.Endpoint,
                    Name = serverName,
                    AverageMatchesPerDay = serverInfo.AverageMatchesPerDay
                });
            }

        }
    }
}
