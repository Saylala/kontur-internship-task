using System.Linq;

namespace Kontur.GameStats.Server.Database
{
    public class RecentMatchesUpdater
    {
        public void Update(MatchInfo info, DatabaseContext databaseContext)
        {
            var recentMatches = databaseContext.RecentMatches.OrderByDescending(x => x.Timestamp).ToList();
            if (recentMatches.Count < 50)
                databaseContext.RecentMatches.Add(
                    new RecentMatch
                    {
                        Server = info.Endpoint,
                        Timestamp = info.Timestamp,
                        Results = info
                    });
            else if (recentMatches[recentMatches.Count - 1].Timestamp < info.Timestamp)
            {
                databaseContext.RecentMatches.Remove(recentMatches[recentMatches.Count - 1]);
                databaseContext.RecentMatches.Add(
                    new RecentMatch
                    {
                        Server = info.Endpoint,
                        Timestamp = info.Timestamp,
                        Results = info
                    });
            }
        }
    }
}
