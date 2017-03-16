using System;

namespace Kontur.GameStats.Server.Models.DatabaseEntries
{
    public class DayCountEntry
    {
        public int Id { get; set; }
        public DateTime Day { get; set; }
        public int Count { get; set; }
    }
}
