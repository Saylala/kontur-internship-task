using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Kontur.GameStats.Server.Models
{
    public class MatchInfo
    {
        [Key]
        [JsonIgnore]
        public string Key { get; set; }
        [JsonIgnore]
        public string Endpoint { get; set; }
        [JsonIgnore]
        public DateTime Timestamp { get; set; }

        public string Map { get; set; }
        public string GameMode { get; set; }
        public int FragLimit { get; set; }
        public int TimeLimit { get; set; }
        public double TimeElapsed { get; set; }
        public virtual List<Score> Scoreboard { get; set; }

        [JsonIgnore]
        public virtual RecentMatch RecentMatch { get; set; }
    }
}
