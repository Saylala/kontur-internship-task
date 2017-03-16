using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kontur.GameStats.Server.Models.DatabaseEntries
{
    [Table("Servers")]
    public class ServerInfoEntry
    {
        [Key]
        public string Endpoint { get; set; }

        public string Name { get; set; }
        public virtual List<StringEntry> GameModes { get; set; }
    }
}
