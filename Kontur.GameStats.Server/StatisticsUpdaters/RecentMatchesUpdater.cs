using System.Data.Entity;
using System.Linq;
using Kontur.GameStats.Server.Database;
using Kontur.GameStats.Server.Models;

namespace Kontur.GameStats.Server.StatisticsUpdaters
{
    public class RecentMatchesUpdater : IStatisticsUpdater
    {
        public void Update(MatchInfo info)
        {
            const int maxServersCount = 50;

            using (var databaseContext = new DatabaseContext())
            {
                foreach (var entry in databaseContext.ChangeTracker.Entries<MatchInfo>())
                    entry.State = EntityState.Unchanged;
                var recentMatches = databaseContext.RecentMatches.OrderByDescending(x => x.Timestamp).ToList();

                if (recentMatches.Count < maxServersCount)
                    databaseContext.RecentMatches.Add(
                        new RecentMatch
                        {
                            MatchInfoKey = info.Key,
                            Server = info.Endpoint,
                            Timestamp = info.Timestamp,
                        });
                else if (recentMatches[recentMatches.Count - 1].Timestamp < info.Timestamp)
                {
                    databaseContext.RecentMatches.Remove(recentMatches[recentMatches.Count - 1]);
                    databaseContext.RecentMatches.Add(
                        new RecentMatch
                        {
                            MatchInfoKey = info.Key,
                            Server = info.Endpoint,
                            Timestamp = info.Timestamp,
                        });
                }

                databaseContext.SaveChanges();
            }
        }
    }
}
