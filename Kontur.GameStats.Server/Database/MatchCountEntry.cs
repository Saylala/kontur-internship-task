using System;
using System.ComponentModel.DataAnnotations;

namespace Kontur.GameStats.Server.Database
{
    public class MatchCountEntry
    {
        [Key]
        public string Key { get; set; }
        public string Endpoint { get; set; }
        public DateTime TimeStamp { get; set; }
        public int Count { get; set; }
    }
}
