using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Globalization;
using System.Linq;
using Kontur.GameStats.Server.Exceptions;
using Kontur.GameStats.Server.Models;
using Kontur.GameStats.Server.StatisticsUpdaters;

namespace Kontur.GameStats.Server.Database
{
    public class GameStatistics
    {
        private readonly DatabaseContext databaseContext;
        private readonly StatisticsUpdater statisticsUpdater;

        public GameStatistics()
        {
            databaseContext = new DatabaseContext();
            statisticsUpdater = new StatisticsUpdater();
        }

        public void PutServerInfo(string endpoint, ServerInfo infoEntry)
        {
            infoEntry.Endpoint = endpoint;

            databaseContext.Set<ServerInfo>().AddOrUpdate(infoEntry);
            databaseContext.SaveChanges();
        }

        public ServerInfo GetServerInfo(string endpoint)
        {
            var entry = databaseContext.Servers.Find(endpoint);
            if (entry == null)
                throw new NotFoundException("Entry not found");
            return entry;
        }

        public void PutMatchInfo(string endpoint, DateTime timestamp, MatchInfo info)
        {
            var serverEntry = databaseContext.Servers.Find(endpoint);
            if (serverEntry == null)
                throw new BadRequestException("Bad Request");

            info.Key = endpoint + timestamp.ToString(CultureInfo.InvariantCulture);
            info.Endpoint = endpoint;
            info.Timestamp = timestamp;

            databaseContext.Matches.Add(info);
            databaseContext.SaveChanges();

            statisticsUpdater.Update(info);
        }

        public MatchInfo GetMatchInfo(string endpoint, DateTime timestamp)
        {
            var entry = databaseContext.Matches.Find(endpoint, timestamp);
            if (entry == null)
                throw new NotFoundException("Entry not found");
            return entry;
        }

        public List<ServerInfo> GetServersInfo()
        {
            return databaseContext.Servers
                .ToList();
        }

        public ServerStatistics GetServerStatistics(string endpoint)
        {
            var entry = databaseContext.ServerStatistics.Find(endpoint);
            if (entry == null)
                throw new NotFoundException("Entry not found");
            return entry;
        }

        public PlayerStatistics GetPlayerStatistics(string name)
        {
            var entry = databaseContext.PlayersStatistics.Find(name);
            if (entry == null)
                throw new NotFoundException("Entry not found");
            return entry;
        }

        public List<RecentMatch> GetRecentMatches(int count)
        {
            return databaseContext.RecentMatches
                .OrderByDescending(x => x.Timestamp)
                .Take(count)
                .ToList();
        }

        public List<BestPlayer> GetBestPlayers(int count)
        {
            return databaseContext.BestPlayers
                .OrderByDescending(x => x.KillToDeathRatio)
                .Take(count)
                .ToList();
        }

        public List<PopularServer> GetPopularServers(int count)
        {
            return databaseContext.PopularServers
                .OrderByDescending(x => x.AverageMatchesPerDay)
                .Take(count)
                .ToList();
        }
    }
}
