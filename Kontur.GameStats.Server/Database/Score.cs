namespace Kontur.GameStats.Server.Models
{
    public class Score
    {
        public int ScoreId { get; set; }
        public string Name { get; set; }
        public int Frags { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int MatchInfoEntryId { get; set; }
    }
}
