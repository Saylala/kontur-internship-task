using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kontur.GameStats.Server.Models
{
    public class ServerInfo
    {
        [Key]
        public string Endpoint { get; set; }

        public string Name { get; set; }
        public virtual List<StringEntry> GameModes { get; set; }
    }
}
