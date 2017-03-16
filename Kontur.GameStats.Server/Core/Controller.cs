using System;
using System.Collections.Generic;
using System.Linq;
using Kontur.GameStats.Server.Database;
using Kontur.GameStats.Server.Models.DatabaseEntries;
using Kontur.GameStats.Server.Models.Serialization;
using Kontur.GameStats.Server.Routing.Attributes;

using ServerStatistics = Kontur.GameStats.Server.Models.Serialization.ServerStatistics;
using StringEntry = Kontur.GameStats.Server.Models.DatabaseEntries.StringEntry;

namespace Kontur.GameStats.Server.Core
{
    public class Controller
    {
        private readonly GameStatistics statistics = new GameStatistics();

        [Put]
        [Route("/servers/<endpoint>/info")]
        public void PutServerInfo(ServerInfo serverInfo, string endpoint)
        {
            statistics.PutServerInfo(endpoint, new ServerInfoEntry
            {
                Name = serverInfo.Name,
                GameModes = serverInfo.GameModes.Select(x => new StringEntry { String = x}).ToList()
            });
        }

        [Route("/servers/<endpoint>/info")]
        public ServerInfo GetServerInfo(string endpoint)
        {
            return statistics.GetServerInfo(endpoint);
        }

        [Put]
        [Route("/servers/<endpoint>/matches/<timestamp>")]
        public void PutMatchInfo(MatchInfoEntry serverInfoEntry, string endpoint, DateTime timestamp)
        {
            statistics.PutMatchInfo(endpoint, timestamp, serverInfoEntry);
        }

        [Route("/servers/<endpoint>/matches/<timestamp>")]
        public MatchInfo GetMatchInfo(string endpoint, DateTime timestamp)
        {
            return statistics.GetMatchInfo(endpoint, timestamp);
        }

        [Route("/servers/info")]
        public List<ServersInfo> GetServersInfo()
        {
            return statistics.GetServersInfo();
        }

        [Route("/servers/<endpoint>/stats")]
        public ServerStatistics GetServerStatisctics(string endpoint)
        {
            return statistics.GetServerStatistics(endpoint);
        }

        [Route("/players/<name>/stats")]
        public PlayerStatistics GetPlayerStatisctics(string name)
        {
            return statistics.GetPlayerStatistics(name);
        }

        [Route("/reports/recent-matches[/<count>]")]
        public List<RecentMatch> GetRecentMatchesInfo(int count = 5)
        {
            return statistics.GetRecentMatches(Clamp(count));
        }

        [Route("/reports/best-players[/<count>]")]
        public List<BestPlayer> GetBestPlayersInfo(int count = 5)
        {
            return statistics.GetBestPlayers(Clamp(count));
        }

        [Route("/reports/popular-servers[/<count>]")]
        public List<PopularServer> GetPopularServersInfo(int count = 5)
        {
            return statistics.GetPopularServers(Clamp(count));
        }

        private static int Clamp(int value, int left = 0, int right = 50)
        {
            return value < left ? left : (value > right ? right : value);
        }
    }
}
