using Kontur.GameStats.Server.Models.DatabaseEntries;

namespace Kontur.GameStats.Server.Models.Serialization
{
    public class BestPlayer
    {
        public BestPlayer()
        {
        }

        public BestPlayer(BestPlayerEntry bestPlayerEntry)
        {
            Name = bestPlayerEntry.Name;
            KillToDeathRatio = bestPlayerEntry.KillToDeathRatio;
        }

        public string Name { get; set; }
        public double KillToDeathRatio { get; set; }
    }
}
