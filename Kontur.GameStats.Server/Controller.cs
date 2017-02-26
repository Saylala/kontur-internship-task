using System;
using Kontur.GameStats.Server.Attributes;
using Kontur.GameStats.Server.Database;
using Newtonsoft.Json;

namespace Kontur.GameStats.Server
{
    public class Controller
    {
        public Controller()
        {
        }

        //Deserialize не здесь??
        [Put]
        [Match("^/servers/(.+?)/info$")]
        public void PutServerInfo(string endpoint, ServerInfo info)
        {
            databaseContext.PutServerInfo(endpoint, info);
        }

        [Match("^/servers/(.+?)/info$")]
        public string GetServerInfo(string endpoint)
        {
            return JsonConvert.SerializeObject(databaseContext.GetServerInfo(endpoint));
        }

        [Put]
        [Match("^/servers/(.+?)/matches/(.+?)$")]
        public void PutMatchInfo(string endpoint, DateTime timestamp, MatchInfo info)
        {
            databaseContext.PutMatchInfo(endpoint, timestamp, info);
        }

        [Match("^/servers/(.+?)/matches/(.+?)$")]
        public string GetMatchInfo(string endpoint, DateTime timestamp)
        {
            return JsonConvert.SerializeObject(databaseContext.GetMatchInfo(endpoint, timestamp));
        }

        [Match("^/servers/info$")]
        public string GetServersInfo()
        {
            return JsonConvert.SerializeObject(databaseContext.GetServersInfo());
        }

        [Match("^/servers/(.+?)/stats$")]
        public string GetServerStatisctics(string endpoint)
        {
            return JsonConvert.SerializeObject(databaseContext.GetServerStatistics(endpoint));
        }

        [Match("^/players/(.+?)/stats$")]
        public string GetPlayerStatisctics(string name)
        {
            return JsonConvert.SerializeObject(databaseContext.GetPlayerStatistics(name));
        }

        [Match("^/reports/recent-matches(.*?)$")]
        public string GetRecentMatchesInfo(string count)
        {
            throw new NotImplementedException();
        }

        [Match("^/reports/best-players(.*?)$")]
        public string GetBestPlayersInfo(string count)
        {
            throw new NotImplementedException();
        }

        [Match("^/reports/popular-servers(.*?)$")]
        public string GetPopularServersInfo(string count)
        {
            throw new NotImplementedException();
        }

        private readonly Database.DatabaseContext databaseContext;
    }
}
