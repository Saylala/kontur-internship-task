using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Kontur.GameStats.Server.Exceptions;
using Kontur.GameStats.Server.Models.DatabaseEntries;
using Kontur.GameStats.Server.Models.Serialization;
using Kontur.GameStats.Server.StatisticsUpdaters;

namespace Kontur.GameStats.Server.Database
{
    public class GameStatistics
    {
        private readonly StatisticsUpdater statisticsUpdater;

        public GameStatistics()
        {
            statisticsUpdater = new StatisticsUpdater();
            using (var databaseContext = new DatabaseContext())
            {
                new DatabaseInitializer().InitializeDatabase(databaseContext);
                databaseContext.SaveChanges();
            }
        }

        public void PutServerInfo(string endpoint, ServerInfoEntry infoEntry)
        {
            using (var databaseContext = new DatabaseContext())
            {
                var entry = databaseContext.Servers.Find(endpoint);
                if (entry != null)
                {
                    databaseContext.StringEntries.RemoveRange(entry.GameModes);
                    databaseContext.Servers.Remove(entry);
                }


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
                return new ServerInfo(entry);
            }
        }

        public void PutMatchInfo(string endpoint, DateTime timestamp, MatchInfoEntry infoEntry)
        {
            using (var databaseContext = new DatabaseContext())
            {
                var serverEntry = databaseContext.Servers.Find(endpoint);
                if (serverEntry == null)
                    throw new BadRequestException("Bad Request");

                infoEntry.Key = endpoint + timestamp.ToString(CultureInfo.InvariantCulture);
                infoEntry.Endpoint = endpoint;
                infoEntry.Timestamp = timestamp;

                databaseContext.Matches.Add(infoEntry);
                databaseContext.SaveChanges();
            }

            statisticsUpdater.Update(infoEntry);
        }

        // todo fix keys(make composit key)
        public MatchInfo GetMatchInfo(string endpoint, DateTime timestamp)
        {
            using (var databaseContext = new DatabaseContext())
            {
                var entry = databaseContext.Matches.Find(endpoint+timestamp.ToString(CultureInfo.InvariantCulture));
                if (entry == null)
                    throw new NotFoundException("Entry not found");
                return new MatchInfo(entry);
            }
        }

        public List<ServersInfo> GetServersInfo()
        {
            using (var databaseContext = new DatabaseContext())
            {
                return databaseContext.Servers
                    .ToList()
                    .Select(x => new ServersInfo(x))
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
                return new ServerStatistics(entry);
            }
        }

        public PlayerStatistics GetPlayerStatistics(string name)
        {
            using (var databaseContext = new DatabaseContext())
            {
                var entry = databaseContext.PlayersStatistics.Find(name);
                if (entry == null)
                    throw new NotFoundException("Entry not found");
                return new PlayerStatistics(entry);
            }
        }

        public List<RecentMatch> GetRecentMatches(int count)
        {
            using (var databaseContext = new DatabaseContext())
            {
                return databaseContext.RecentMatches
                    .Take(count)
                    .ToList()
                    .OrderByDescending(x => x.Timestamp)
                    .Select(x => new RecentMatch(x))
                    .ToList();
            }
        }

        public List<BestPlayer> GetBestPlayers(int count)
        {
            using (var databaseContext = new DatabaseContext())
            {
                return databaseContext.BestPlayers
                    .Take(count)
                    .ToList()
                    .OrderByDescending(x => x.KillToDeathRatio)
                    .Select(x => new BestPlayer(x))
                    .ToList();
            }
        }

        public List<PopularServer> GetPopularServers(int count)
        {
            using (var databaseContext = new DatabaseContext())
            {
                return databaseContext.PopularServers
                    .Take(count)
                    .ToList()
                    .OrderByDescending(x => x.AverageMatchesPerDay)
                    .Select(x => new PopularServer(x))
                    .ToList();
            }
        }
    }
}
