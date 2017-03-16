using Kontur.GameStats.Server.Models.DatabaseEntries;

namespace Kontur.GameStats.Server.Models.Serialization
{
    public class PopularServer
    {
        public PopularServer()
        {
        }

        public PopularServer(PopularServerEntry popularServerEntry)
        {
            Endpoint = popularServerEntry.Endpoint;
            Name = popularServerEntry.Name;
            AverageMatchesPerDay = popularServerEntry.AverageMatchesPerDay;
        }

        public string Endpoint { get; set; }
        public string Name { get; set; }
        public double AverageMatchesPerDay { get; set; }
    }
}
