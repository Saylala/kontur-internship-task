using System.Data.Entity;
using System.Linq;
using Kontur.GameStats.Server.Database;
using Kontur.GameStats.Server.Models.DatabaseEntries;

namespace Kontur.GameStats.Server.StatisticsUpdaters
{
    public class RecentMatchesUpdater : IStatisticsUpdater
    {
        public void Update(MatchInfoEntry infoEntry, DatabaseContext databaseContext)
        {
            const int maxServersCount = 50;

            foreach (var entry in databaseContext.ChangeTracker.Entries<MatchInfoEntry>())
                entry.State = EntityState.Unchanged;
            var recentMatches = databaseContext.RecentMatches.OrderByDescending(x => x.Timestamp).ToList();

            if (recentMatches.Count < maxServersCount)
                databaseContext.RecentMatches.Add(
                    new RecentMatchEntry
                    {
                        Key = infoEntry.Key,
                        Server = infoEntry.Endpoint,
                        Timestamp = infoEntry.Timestamp,
                    });
            else if (recentMatches[recentMatches.Count - 1].Timestamp < infoEntry.Timestamp)
            {
                databaseContext.RecentMatches.Remove(recentMatches[recentMatches.Count - 1]);
                databaseContext.RecentMatches.Add(
                    new RecentMatchEntry
                    {
                        Key = infoEntry.Key,
                        Server = infoEntry.Endpoint,
                        Timestamp = infoEntry.Timestamp,
                    });
            }

        }
    }
}
