using System.Linq;
using Kontur.GameStats.Server.Database;
using Kontur.GameStats.Server.Models.DatabaseEntries;

namespace Kontur.GameStats.Server.StatisticsUpdaters
{
    public class PopularServersUpdater : IStatisticsUpdater
    {
        public void Update(MatchInfoEntry infoEntry, DatabaseContext databaseContext)
        {
            const int maxServersCount = 50;

            var popularServers = databaseContext.PopularServers.OrderByDescending(x => x.AverageMatchesPerDay).ToList();
            var serverStatistics = databaseContext.ServerStatistics.Find(infoEntry.Endpoint);
            var serverInfo = databaseContext.Servers.Find(infoEntry.Endpoint);
            if (serverStatistics == null || serverInfo == null)
                return;

            var serverName = serverInfo.Name;
            var previous = popularServers.Find(x => x.Endpoint == serverInfo.Endpoint);

            if (previous != null)
                previous.AverageMatchesPerDay = serverStatistics.AverageMatchesPerDay;
            else if (popularServers.Count < maxServersCount)
                databaseContext.PopularServers.Add(new PopularServerEntry
                {
                    Endpoint = infoEntry.Endpoint,
                    Name = serverName,
                    AverageMatchesPerDay = serverStatistics.AverageMatchesPerDay
                });
            else if (popularServers[popularServers.Count - 1].AverageMatchesPerDay < serverStatistics.AverageMatchesPerDay)
            {
                databaseContext.PopularServers.Remove(popularServers[popularServers.Count - 1]);
                databaseContext.PopularServers.Add(new PopularServerEntry
                {
                    Endpoint = infoEntry.Endpoint,
                    Name = serverName,
                    AverageMatchesPerDay = serverStatistics.AverageMatchesPerDay
                });
            }

        }
    }
}
