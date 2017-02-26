using System.ComponentModel.DataAnnotations;

namespace Kontur.GameStats.Server.Database
{
    public class BestPlayer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double KillToDeathRatio { get; set; }
    }
}
