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
            var serverInfo = databaseContext.ServerStatistics.Find(infoEntry.Endpoint);

            if (serverInfo == null)
            {
                return;
            }

            var serverName = databaseContext.Servers.Find(infoEntry.Endpoint).Name;
            var previous = popularServers.Find(x => x.Name == serverName);

            if (previous != null)
                previous.AverageMatchesPerDay = serverInfo.AverageMatchesPerDay;
            else if (popularServers.Count < maxServersCount)
                databaseContext.PopularServers.Add(new PopularServerEntry
                {
                    Endpoint = infoEntry.Endpoint,
                    Name = serverName,
                    AverageMatchesPerDay = serverInfo.AverageMatchesPerDay
                });
            else if (popularServers[popularServers.Count - 1].AverageMatchesPerDay < serverInfo.AverageMatchesPerDay)
            {
                databaseContext.PopularServers.Remove(popularServers[popularServers.Count - 1]);
                databaseContext.PopularServers.Add(new PopularServerEntry
                {
                    Endpoint = infoEntry.Endpoint,
                    Name = serverName,
                    AverageMatchesPerDay = serverInfo.AverageMatchesPerDay
                });
            }

        }
    }
}
