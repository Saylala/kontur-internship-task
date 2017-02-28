using System;
using System.Linq;
using Kontur.GameStats.Server.Attributes;
using Kontur.GameStats.Server.Database;
using Kontur.GameStats.Server.Exceptions;
using Kontur.GameStats.Server.Models;
using Newtonsoft.Json;

namespace Kontur.GameStats.Server.Core
{
    public class Controller
    {
        private readonly GameStatistics statistics;

        public Controller()
        {
            statistics = new GameStatistics();
        }

        [Put]
        [Regex("/servers/(.+)/info")]
        public void PutServerInfo(string endpoint, string info)
        {
            var serverInfo = JsonConvert.DeserializeObject<Models.Serialization.ServerInfo>(info);
            var model = new ServerInfo
            {
                Name = serverInfo.Name,
                GameModes = serverInfo.GameModes.Select(x => new StringEntry {String = x}).ToList()
            };

            statistics.PutServerInfo(endpoint, model);
        }

        [Regex("/servers/(.+)/info")]
        public string GetServerInfo(string endpoint)
        {
            var info = statistics.GetServerInfo(endpoint);

            return JsonConvert.SerializeObject(info);
        }

        [Put]
        [Regex("/servers/(.+)/matches/(.+)")]
        public void PutMatchInfo(string endpoint, string timeStamp, string info)
        {
            var serverInfo = JsonConvert.DeserializeObject<MatchInfo>(info);
            var time = JsonConvert.DeserializeObject<DateTime>(timeStamp);

            statistics.PutMatchInfo(endpoint, time, serverInfo);
        }

        [Regex("/servers/(.+)/matches/(.+)")]
        public string GetMatchInfo(string endpoint, string timeStamp)
        {
            var time = JsonConvert.DeserializeObject<DateTime>(timeStamp);
            var info = statistics.GetMatchInfo(endpoint, time);

            return JsonConvert.SerializeObject(info);
        }

        [Regex("/servers/info")]
        public string GetServersInfo()
        {
            var info = statistics.GetServersInfo();

            return JsonConvert.SerializeObject(info);
        }

        [Regex("/servers/(.+)/stats")]
        public string GetServerStatisctics(string endpoint)
        {
            var info = statistics.GetServerStatistics(endpoint);
            var model = new Models.Serialization.ServerStatistics(info);

            return JsonConvert.SerializeObject(model);
        }

        [Regex("/players/(.+)/stats")]
        public string GetPlayerStatisctics(string name)
        {
            var info = statistics.GetPlayerStatistics(name);

            return JsonConvert.SerializeObject(info);
        }

        [Regex("/reports/recent-matches($|/.+)")]
        public string GetRecentMatchesInfo(string count)
        {
            var entryCount = ProcessCount(count);
            var matches = statistics.GetRecentMatches(entryCount);

            return JsonConvert.SerializeObject(matches);
        }

        [Regex("/reports/best-players($|/.+)")]
        public string GetBestPlayersInfo(string count)
        {
            var entryCount = ProcessCount(count);
            var players = statistics.GetBestPlayers(entryCount);

            return JsonConvert.SerializeObject(players);
        }

        [Regex("/reports/popular-servers($|/.+)")]
        public string GetPopularServersInfo(string count)
        {
            var entryCount = ProcessCount(count);
            var servers = statistics.GetPopularServers(entryCount);

            return JsonConvert.SerializeObject(servers);
        }

        private int ProcessCount(string count)
        {
            const int defaultValue = 5;
            if (count.Length == 0)
                return defaultValue;

            int number;
            var str = count.Substring(1);
            var success = int.TryParse(str, out number);
            if (!success)
                throw new InvalidRequestException("Invalid Request");

            if (number < 0)
                return 0;
            return number > 50 ? 50 : number;
        }
    }
}
