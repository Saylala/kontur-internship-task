using Kontur.GameStats.Server.Models.DatabaseEntries;

namespace Kontur.GameStats.Server.Models.Serialization
{
    public class Score
    {
        public Score()
        {
        }

        public Score(ScoreEntry scoreEntry)
        {
            Name = scoreEntry.Name;
            Frags = scoreEntry.Frags;
            Kills = scoreEntry.Kills;
            Deaths = scoreEntry.Deaths;
        }

        public string Name { get; set; }
        public int Frags { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
    }
}
