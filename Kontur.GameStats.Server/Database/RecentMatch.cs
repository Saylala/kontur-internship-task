using System;

namespace Kontur.GameStats.Server.Database
{
    public class RecentMatch
    {
        public int RecentMatchId { get; set; }
        public string Server { get; set; }
        public DateTime Timestamp { get; set; }
        public int MatchInfoEntryId { get; set; }
        public virtual MatchInfo Results { get; set; }
    }
}
