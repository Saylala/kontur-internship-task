using Kontur.GameStats.Server.Models.DatabaseEntries;

namespace Kontur.GameStats.Server.Models.Serialization
{
    public class ServersInfo
    {
        public ServersInfo()
        {
            
        }

        public ServersInfo(ServerInfoEntry serverInfoEntry)
        {
            Endpoint = serverInfoEntry.Endpoint;
            Info = new ServerInfo(serverInfoEntry);
        }

        public string Endpoint { get; set; }
        public ServerInfo Info { get; set; }
    }
}
