using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kontur.GameStats.Server.Models.DatabaseEntries
{
    [Table("RecentMatches")]
    public class RecentMatchEntry
    {
        [Key, ForeignKey("MatchInfoEntry")]
        public string Key { get; set; }

        public string Server { get; set; }
        public DateTime Timestamp { get; set; }
        public virtual MatchInfoEntry MatchInfoEntry { get; set; }
    }
}
