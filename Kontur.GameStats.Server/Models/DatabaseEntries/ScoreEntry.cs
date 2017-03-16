using System.ComponentModel.DataAnnotations.Schema;

namespace Kontur.GameStats.Server.Models.DatabaseEntries
{
    [Table("Scores")]
    public class ScoreEntry
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public int Frags { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
    }
}
