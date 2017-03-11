using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Kontur.GameStats.Server.Exceptions;
using Kontur.GameStats.Server.Models;
using Kontur.GameStats.Server.StatisticsUpdaters;

namespace Kontur.GameStats.Server.Database
{
    public class GameStatistics
    {
        private readonly StatisticsUpdater statisticsUpdater;

        private readonly SortedList<DateTime, RecentMatch> recentMatches;
        private readonly SortedList<double, BestPlayer> bestPlayers;
        private readonly SortedList<double, PopularServer> popularServers;


        public GameStatistics()
        {
            statisticsUpdater = new StatisticsUpdater(recentMatches, bestPlayers, popularServers);
            using (var databaseContext = new DatabaseContext())
            {
                new DatabaseInitializer().InitializeDatabase(databaseContext);
                databaseContext.SaveChanges();
            }
        }

        public void PutServerInfo(string endpoint, ServerInfo infoEntry)
        {
            using (var databaseContext = new DatabaseContext())
            {
                infoEntry.Endpoint = endpoint;

                databaseContext.Servers.Add(infoEntry);
                databaseContext.SaveChanges();
            }
        }

        public ServerInfo GetServerInfo(string endpoint)
        {
            using (var databaseContext = new DatabaseContext())
            {
                var entry = databaseContext.Servers.Find(endpoint);
                if (entry == null)
                    throw new NotFoundException("Entry not found");
                return entry;
            }
        }

        public void PutMatchInfo(string endpoint, DateTime timestamp, MatchInfo info)
        {
            using (var databaseContext = new DatabaseContext())
            {
                var serverEntry = databaseContext.Servers.Find(endpoint);
                if (serverEntry == null)
                    throw new BadRequestException("Bad Request");

                info.Key = endpoint + timestamp.ToString(CultureInfo.InvariantCulture);
                info.Endpoint = endpoint;
                info.Timestamp = timestamp;

                databaseContext.Matches.Add(info);

                statisticsUpdater.Update(info, databaseContext);

                databaseContext.SaveChanges();
            }
        }

        public MatchInfo GetMatchInfo(string endpoint, DateTime timestamp)
        {
            using (var databaseContext = new DatabaseContext())
            {
                var entry = databaseContext.Matches.Find(endpoint, timestamp);
                if (entry == null)
                    throw new NotFoundException("Entry not found");
                return entry;
            }
        }

        public List<ServerInfo> GetServersInfo()
        {
            using (var databaseContext = new DatabaseContext())
            {
                return databaseContext.Servers
                    .ToList();
            }
        }

        public ServerStatistics GetServerStatistics(string endpoint)
        {
            using (var databaseContext = new DatabaseContext())
            {
                var entry = databaseContext.ServerStatistics.Find(endpoint);
                if (entry == null)
                    throw new NotFoundException("Entry not found");
                return entry;
            }
        }

        public PlayerStatistics GetPlayerStatistics(string name)
        {
            using (var databaseContext = new DatabaseContext())
            {
                var entry = databaseContext.PlayersStatistics.Find(name);
                if (entry == null)
                    throw new NotFoundException("Entry not found");
                return entry;
            }
        }

        public List<RecentMatch> GetRecentMatches(int count)
        {
            return (List<RecentMatch>) recentMatches.Values;
        }

        public List<BestPlayer> GetBestPlayers(int count)
        {
            return (List<BestPlayer>) bestPlayers.Values;
        }

        public List<PopularServer> GetPopularServers(int count)
        {
            return (List<PopularServer>) popularServers.Values;
        }
    }
}
