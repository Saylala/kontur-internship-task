using System.Linq;

namespace Kontur.GameStats.Server.Database
{
    public class PopularServersUpdater
    {
        public void Update(MatchInfo info, DatabaseContext databaseContext)
        {
            var popularServers = databaseContext.PopularServers.OrderByDescending(x => x.AverageMatchesPerDay).ToList();
            var serverInfo = databaseContext.ServerStatistics.Find(info.Endpoint);
            var serverName = databaseContext.Servers.Find(info.Endpoint).Name;
            if (popularServers.Count < 50)
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
