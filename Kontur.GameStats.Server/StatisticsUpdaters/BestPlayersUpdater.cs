using System;
using System.Linq;
using Kontur.GameStats.Server.Database;
using Kontur.GameStats.Server.Models.DatabaseEntries;

namespace Kontur.GameStats.Server.StatisticsUpdaters
{
    public class BestPlayersUpdater : IStatisticsUpdater
    {
        public void Update(MatchInfoEntry infoEntry, DatabaseContext databaseContext)
        {
            const int maxPlayersCount = 50;
            const int requiredMatchesCount = 10;

            var bestPlayers = databaseContext.BestPlayers.OrderByDescending(x => x.KillToDeathRatio).ToList();
            foreach (var player in infoEntry.Scoreboard)
            {
                var playerInfo = databaseContext.PlayersStatistics.FirstOrDefault(x => x.Name.Equals(player.Name, StringComparison.InvariantCultureIgnoreCase));
                if (playerInfo == null || playerInfo.TotalDeaths == 0 ||
                    playerInfo.TotalMatchesPlayed < requiredMatchesCount)
                    continue;

                var previous = bestPlayers.Find(x => x.Name == player.Name);
                if (previous != null)
                {
                    previous.KillToDeathRatio = playerInfo.KillToDeathRatio;
                    continue;
                }
                if (bestPlayers.Count < maxPlayersCount)
                    databaseContext.BestPlayers.Add(new BestPlayerEntry
                    {
                        Name = playerInfo.Name,
                        KillToDeathRatio = playerInfo.KillToDeathRatio
                    });
                else if (bestPlayers[bestPlayers.Count - 1].KillToDeathRatio < playerInfo.KillToDeathRatio)
                {
                    databaseContext.BestPlayers.Remove(bestPlayers[bestPlayers.Count - 1]);
                    databaseContext.BestPlayers.Add(new BestPlayerEntry
                    {
                        Name = playerInfo.Name,
                        KillToDeathRatio = playerInfo.KillToDeathRatio
                    });
                }
            }
        }
    }
}
