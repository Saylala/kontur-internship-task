using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Kontur.GameStats.Server.Models
{
    public class RecentMatch
    {
        [Key, ForeignKey("Results")]
        [JsonIgnore]
        public string MatchInfoKey { get; set; }

        public string Server { get; set; }
        public DateTime Timestamp { get; set; }
        public virtual MatchInfo Results { get; set; }
    }
}
