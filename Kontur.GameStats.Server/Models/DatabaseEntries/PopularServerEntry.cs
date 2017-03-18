using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kontur.GameStats.Server.Models.DatabaseEntries
{
    [Table("PopularServers")]
    public class PopularServerEntry
    {
        [Key]
        public string Endpoint { get; set; }

        public string Name { get; set; }
        public double AverageMatchesPerDay { get; set; }
    }
}
