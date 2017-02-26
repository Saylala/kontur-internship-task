using System.Linq;

namespace Kontur.GameStats.Server.Database
{
    public class BestPlayersUpdater
    {
        public void Update(MatchInfo info, DatabaseContext databaseContext)
        {
            var bestPlayers = databaseContext.BestPlayers.OrderByDescending(x => x.KillToDeathRatio).ToList();
            foreach (var player in info.Scoreboard)
            {
                var playerInfo = databaseContext.PlayersStatistics.Find(player.Name);
                if (bestPlayers.Count < 50 && bestPlayers.All(x => x.Name != player.Name))
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
        }
    }
}
