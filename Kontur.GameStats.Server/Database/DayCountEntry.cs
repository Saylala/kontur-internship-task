using System;
using System.ComponentModel.DataAnnotations;

namespace Kontur.GameStats.Server.Database
{
    public class DayCountEntry
    {
        public int Id { get; set; }
        public DateTime Day { get; set; }
        public int Count { get; set; }
    }
}
