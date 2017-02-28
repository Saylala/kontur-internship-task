using System;

namespace Kontur.GameStats.Server.Models
{
    public class DayCountEntry
    {
        public int Id { get; set; }
        public DateTime Day { get; set; }
        public int Count { get; set; }
    }
}
