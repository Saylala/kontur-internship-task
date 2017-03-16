using System;
using Kontur.GameStats.Server.Models.DatabaseEntries;

namespace Kontur.GameStats.Server.Models.Serialization
{
    public class RecentMatch
    {
        public RecentMatch()
        {
        }

        public RecentMatch(RecentMatchEntry recentMatchEntry)
        {
            Server = recentMatchEntry.Server;

            Timestamp = DateTime.SpecifyKind(recentMatchEntry.Timestamp, DateTimeKind.Utc);

            Results = new MatchInfo(recentMatchEntry.MatchInfoEntry);
        }

        public string Server { get; set; }
        public DateTime Timestamp { get; set; }
        public MatchInfo Results { get; set; }
    }
}
