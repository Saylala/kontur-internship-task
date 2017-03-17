using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public async Task PutServerInfo(ServerInfo serverInfo, string endpoint)
        {
            await statistics.PutServerInfo(endpoint, new ServerInfoEntry
            {
                Name = serverInfo.Name,
                GameModes = serverInfo.GameModes.Select(x => new StringEntry { String = x}).ToList()
            });
        }

        [Route("/servers/<endpoint>/info")]
        public async Task<ServerInfo> GetServerInfo(string endpoint)
        {
            return await statistics.GetServerInfo(endpoint);
        }

        [Put]
        [Route("/servers/<endpoint>/matches/<timestamp>")]
        public async Task PutMatchInfo(MatchInfoEntry serverInfoEntry, string endpoint, DateTime timestamp)
        {
            await statistics.PutMatchInfo(endpoint, timestamp, serverInfoEntry);
        }

        [Route("/servers/<endpoint>/matches/<timestamp>")]
        public async Task<MatchInfo> GetMatchInfo(string endpoint, DateTime timestamp)
        {
            return await statistics.GetMatchInfo(endpoint, timestamp);
        }

        [Route("/servers/info")]
        public async Task<List<ServersInfo>> GetServersInfo()
        {
            return await statistics.GetServersInfo();
        }

        [Route("/servers/<endpoint>/stats")]
        public async Task<ServerStatistics> GetServerStatisctics(string endpoint)
        {
            return await statistics.GetServerStatistics(endpoint);
        }

        [Route("/players/<name>/stats")]
        public async Task<PlayerStatistics> GetPlayerStatisctics(string name)
        {
            return await statistics.GetPlayerStatistics(name);
        }

        [Route("/reports/recent-matches[/<count>]")]
        public async Task<List<RecentMatch>> GetRecentMatchesInfo(int count = 5)
        {
            return await statistics.GetRecentMatches(Clamp(count));
        }

        [Route("/reports/best-players[/<count>]")]
        public async Task<List<BestPlayer>> GetBestPlayersInfo(int count = 5)
        {
            return await statistics.GetBestPlayers(Clamp(count));
        }

        [Route("/reports/popular-servers[/<count>]")]
        public async Task<List<PopularServer>> GetPopularServersInfo(int count = 5)
        {
            return await statistics.GetPopularServers(Clamp(count));
        }

        private static int Clamp(int value, int left = 0, int right = 50)
        {
            return value < left ? left : (value > right ? right : value);
        }
    }
}
