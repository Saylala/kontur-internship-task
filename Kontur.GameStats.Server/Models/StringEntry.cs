using Newtonsoft.Json;

namespace Kontur.GameStats.Server.Models
{
    public class StringEntry
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string String { get; set; }
    }
}
