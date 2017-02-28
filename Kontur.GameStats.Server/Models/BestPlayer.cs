using Newtonsoft.Json;

namespace Kontur.GameStats.Server.Models
{
    public class BestPlayer
    {
        [JsonIgnore]
        public int Id { get; set; }

        public string Name { get; set; }
        public double KillToDeathRatio { get; set; }
    }
}
