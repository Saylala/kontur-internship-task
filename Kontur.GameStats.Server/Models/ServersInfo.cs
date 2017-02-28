using Kontur.GameStats.Server.Models.Serialization;

namespace Kontur.GameStats.Server.Models
{
    public class ServersInfo
    {
        public string Endpoint { get; set; }
        public Serialization.ServerInfo Info { get; set; }
    }
}
