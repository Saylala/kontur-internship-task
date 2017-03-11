using System.Collections.Generic;
using System.Linq;
using Kontur.GameStats.Server.Database;
using Kontur.GameStats.Server.Models;

namespace Kontur.GameStats.Server.StatisticsUpdaters
{
    public class BestPlayersUpdater : IStatisticsUpdater
    {
        private readonly SortedList<double, BestPlayer> bestPlayers;
        private const int maxPlayersCount = 50;
        private const int requiredMatchesCount = 10;

        public BestPlayersUpdater(SortedList<double, BestPlayer> bestPlayers)
        {
            this.bestPlayers = bestPlayers;
        }

        public void Update(MatchInfo info, DatabaseContext databaseContext)
        {
            foreach (var player in info.Scoreboard)
            {
                var playerInfo = databaseContext.PlayersStatistics.Find(player.Name);

                if (playerInfo == null)
                {
                    continue;
                }

                if (playerInfo.TotalDeaths == 0 || playerInfo.TotalMatchesPlayed < requiredMatchesCount)
                    continue;

                //var previous = bestPlayers.FirstOrDefault(x => x.Value.Name == player.Name);
                //if (previous.Value != null)
                //{
                //    previous.. = playerInfo.KillToDeathRatio;
                //    continue;
                //}


                if (bestPlayers.Count < maxPlayersCount)
                    bestPlayers.Add(
                        playerInfo.KillToDeathRatio,
                        new BestPlayer
                        {
                            Name = playerInfo.Name,
                            KillToDeathRatio = playerInfo.KillToDeathRatio
                        });
                else if (bestPlayers.Values[bestPlayers.Count - 1].KillToDeathRatio < playerInfo.KillToDeathRatio)
                {
                    bestPlayers.RemoveAt(bestPlayers.Count - 1);
                    bestPlayers.Add(
                        playerInfo.KillToDeathRatio,
                        new BestPlayer
                        {
                            Name = playerInfo.Name,
                            KillToDeathRatio = playerInfo.KillToDeathRatio
                        });
                }
            }
        }
    }
}
