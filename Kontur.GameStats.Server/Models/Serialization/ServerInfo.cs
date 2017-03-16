using System.Collections.Generic;
using System.Linq;
using Kontur.GameStats.Server.Models.DatabaseEntries;

namespace Kontur.GameStats.Server.Models.Serialization
{
    public class ServerInfo
    {
        public ServerInfo()
        {
        }

        public ServerInfo(ServerInfoEntry entry)
        {
            Name = entry.Name;
            GameModes = entry.GameModes.Select(x => x.String).ToList();
        }

        public string Name { get; set; }
        public List<string> GameModes { get; set; }
    }
}
