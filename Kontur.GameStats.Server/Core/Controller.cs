using System;
using System.Collections.Generic;
using System.Linq;
using Kontur.GameStats.Server.Database;
using Kontur.GameStats.Server.Models;
using Kontur.GameStats.Server.Routing.Attributes;

using ServerStatistics = Kontur.GameStats.Server.Models.Serialization.ServerStatistics;

namespace Kontur.GameStats.Server.Core
{
    public class Controller
    {
        private readonly GameStatistics statistics = new GameStatistics();

        [Put]
        [Route("/servers/<endpoint>/info")]
        public void PutServerInfo(Models.Serialization.ServerInfo serverInfo, string endpoint)
        {
            statistics.PutServerInfo(endpoint, new ServerInfo
            {
                Name = serverInfo.Name,
                GameModes = serverInfo.GameModes.Select(x => new StringEntry {String = x}).ToList()
            });
        }

        [Route("/servers/<endpoint>/info")]
        public ServerInfo GetServerInfo(string endpoint)
        {
            return statistics.GetServerInfo(endpoint);
        }

        [Put]
        [Route("/servers/<endpoint>/matches/<timestamp>")]
        public void PutMatchInfo(MatchInfo serverInfo, string endpoint, DateTime timestamp)
        {
            statistics.PutMatchInfo(endpoint, timestamp, serverInfo);
        }

        [Route("/servers/<endpoint>/matches/<timestamp>")]
        public MatchInfo GetMatchInfo(string endpoint, DateTime timestamp)
        {
            return statistics.GetMatchInfo(endpoint, timestamp);
        }

        [Route("/servers/info")]
        public List<ServerInfo> GetServersInfo()
        {
            return statistics.GetServersInfo();
        }

        [Route("/servers/<endpoint>/stats")]
        public ServerStatistics GetServerStatisctics(string endpoint)
        {
            return new ServerStatistics(statistics.GetServerStatistics(endpoint));
        }

        [Route("/players/<name>/stats")]
        public PlayerStatistics GetPlayerStatisctics(string name)
        {
            return statistics.GetPlayerStatistics(name);
        }

        [Route("/reports/recent-matches[/<count>]")]
        public List<RecentMatch> GetRecentMatchesInfo(int count = 5)
        {
            count = Clamp(0, 50, count);
            return statistics.GetRecentMatches(count);
        }

        [Route("/reports/best-players[/<count>]")]
        public List<BestPlayer> GetBestPlayersInfo(int count = 5)
        {
            count = Clamp(0, 50, count);
            return statistics.GetBestPlayers(count);
        }

        [Route("/reports/popular-servers[/<count>]")]
        public List<PopularServer> GetPopularServersInfo(int count = 5)
        {
            count = Clamp(0, 50, count);
            return statistics.GetPopularServers(count);
        }

        private static int Clamp(int left, int right, int value)
        {
            return value < left ? left : (value > right ? right : value);
        }
    }
}
