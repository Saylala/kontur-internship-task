using System.Collections.Generic;
using System.Linq;
using Kontur.GameStats.Server.Models.DatabaseEntries;

namespace Kontur.GameStats.Server.Models.Serialization
{
    public class MatchInfo
    {
        public MatchInfo()
        {
        }

        public MatchInfo(MatchInfoEntry entry)
        {
            Map = entry.Map;
            GameMode = entry.GameMode;
            FragLimit = entry.FragLimit;
            TimeLimit = entry.TimeLimit;
            TimeElapsed = entry.TimeElapsed;
            Scoreboard = entry.Scoreboard.Select(x => new Score(x)).ToList();
        }

        public string Map { get; set; }
        public string GameMode { get; set; }
        public int FragLimit { get; set; }
        public int TimeLimit { get; set; }
        public double TimeElapsed { get; set; }
        public List<Score> Scoreboard { get; set; }
    }
}
