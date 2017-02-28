using System.Linq;
using Kontur.GameStats.Server.Database;
using Kontur.GameStats.Server.Models;

namespace Kontur.GameStats.Server.StatisticsUpdaters
{
    public class BestPlayersUpdater : IStatisticsUpdater
    {
        public void Update(MatchInfo info)
        {
            const int maxPlayersCount = 50;
            const int requiredMatchesCount = 10;

            using (var databaseContext = new DatabaseContext())
            {
                var bestPlayers = databaseContext.BestPlayers.OrderByDescending(x => x.KillToDeathRatio).ToList();
                foreach (var player in info.Scoreboard)
                {
                    var playerInfo = databaseContext.PlayersStatistics.Find(player.Name);
                    if (playerInfo.TotalDeaths == 0 || playerInfo.TotalMatchesPlayed < requiredMatchesCount)
                        continue;

                    var previous = bestPlayers.Find(x => x.Name == player.Name);
                    if (previous != null)
                    {
                        previous.KillToDeathRatio = playerInfo.KillToDeathRatio;
                        continue;
                    }
                    if (bestPlayers.Count < maxPlayersCount)
                        databaseContext.BestPlayers.Add(new BestPlayer
                        {
                            Name = playerInfo.Name,
                            KillToDeathRatio = playerInfo.KillToDeathRatio
                        });
                    else if (bestPlayers[bestPlayers.Count - 1].KillToDeathRatio < playerInfo.KillToDeathRatio)
                    {
                        databaseContext.BestPlayers.Remove(bestPlayers[bestPlayers.Count - 1]);
                        databaseContext.BestPlayers.Add(new BestPlayer
                        {
                            Name = playerInfo.Name,
                            KillToDeathRatio = playerInfo.KillToDeathRatio
                        });
                    }
                }

                databaseContext.SaveChanges();
            }
        }
    }
}
