using System.ComponentModel.DataAnnotations.Schema;

namespace Kontur.GameStats.Server.Models.DatabaseEntries
{
    [Table("BestPlayers")]
    public class BestPlayerEntry
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double KillToDeathRatio { get; set; }
    }
}
