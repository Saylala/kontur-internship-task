using System.ComponentModel.DataAnnotations;

namespace Kontur.GameStats.Server.Database
{
    public class NameCountEntry
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
    }
}
