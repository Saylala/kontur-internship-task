using System.ComponentModel.DataAnnotations;

namespace Kontur.GameStats.Server.Models
{
    public class PopularServer
    {
        [Key]
        public string Endpoint { get; set; }
        public string Name { get; set; }
        public double AverageMatchesPerDay { get; set; }
    }
}
