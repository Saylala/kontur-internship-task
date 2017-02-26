using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Kontur.GameStats.Server.Models;

namespace Kontur.GameStats.Server.Database
{
    public class MatchInfo
    {
        //public int MatchInfoEntryId { get; set; }
        [Key]
        public string Key { get; set; }
        public string Endpoint { get; set; }
        public DateTime Timestamp { get; set; }
        public string Map { get; set; }
        public string GameMode { get; set; }
        public int FragLimit { get; set; }
        public int TimeLimit { get; set; }
        public double TimeElapsed { get; set; }
        public virtual List<Score> Scoreboard { get; set; }
        //public int RecentMatchEntryId { get; set; }
    }
}
