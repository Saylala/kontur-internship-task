using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kontur.GameStats.Server.Models.DatabaseEntries
{
    [Table("Matches")]
    public class MatchInfoEntry
    {
        [Key]
        public string Key { get; set; }

        public string Endpoint { get; set; }
        public DateTime Timestamp { get; set; }
        public string Map { get; set; }
        public string GameMode { get; set; }
        public int FragLimit { get; set; }
        public int TimeLimit { get; set; }
        public double TimeElapsed { get; set; }

        public virtual List<ScoreEntry> Scoreboard { get; set; }
        public virtual RecentMatchEntry RecentMatchEntry { get; set; }
    }
}
