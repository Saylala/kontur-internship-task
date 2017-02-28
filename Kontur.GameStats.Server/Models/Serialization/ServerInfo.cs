using System.Collections.Generic;

namespace Kontur.GameStats.Server.Models.Serialization
{
    public class ServerInfo
    {
        public string Name { get; set; }
        public List<string> GameModes { get; set; }
    }
}
