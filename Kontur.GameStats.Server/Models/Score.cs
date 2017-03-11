using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Kontur.GameStats.Server.Models
{
    [Table("Scores")]
    public class Score
    {
        [JsonIgnore]
        public int Id { get; set; }

        public string Name { get; set; }
        public int Frags { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }

        //[JsonIgnore]
        //public int MatchInfoEntryId { get; set; }
    }
}
