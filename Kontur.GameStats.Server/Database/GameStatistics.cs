using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Kontur.GameStats.Server.Exceptions;
using Kontur.GameStats.Server.Models.DatabaseEntries;
using Kontur.GameStats.Server.Models.Serialization;
using Kontur.GameStats.Server.StatisticsUpdaters;

namespace Kontur.GameStats.Server.Database
{
    public class GameStatistics
    {
        private readonly StatisticsUpdater statisticsUpdater;
        private readonly DatabaseContext databaseContext;


        public GameStatistics()
        {
            statisticsUpdater = new StatisticsUpdater();
            databaseContext = new DatabaseContext();
            DatabaseInitializer.InitializeDatabase(databaseContext);
            databaseContext.SaveChanges();
        }

        public async Task PutServerInfo(string endpoint, ServerInfoEntry infoEntry)
        {
            await Task.Run(() =>
            {
                infoEntry.Endpoint = endpoint;
                var entry = databaseContext.Servers.Find(endpoint);

                lock (databaseContext)
                {
                    if (entry != null)
                    {
                        databaseContext.StringEntries.RemoveRange(entry.GameModes);
                        databaseContext.Servers.Remove(entry);
                    }
                    databaseContext.Servers.Add(infoEntry);
                    databaseContext.SaveChanges();
                }
            });
        }

        public async Task<ServerInfo> GetServerInfo(string endpoint)
        {
            return await Task.Run(() =>
            {
                var entry = databaseContext.Servers.Find(endpoint);
                if (entry == null)
                    throw new NotFoundException("Entry not found");
                return new ServerInfo(entry);
            });
        }

        public async Task PutMatchInfo(string endpoint, DateTime timestamp, MatchInfoEntry infoEntry)
        {
            await Task.Run(() =>
            {
                var serverEntry = databaseContext.Servers.Find(endpoint);
                if (serverEntry == null)
                    throw new BadRequestException("Bad Request");

                var key = endpoint + timestamp.ToString(CultureInfo.InvariantCulture);
                if (databaseContext.Matches.Find(key) != null)
                    throw new BadRequestException("Bad Request");

                infoEntry.Key = key;
                infoEntry.Endpoint = endpoint;
                infoEntry.Timestamp = timestamp;

                lock (databaseContext)
                {
                    // todo wtf is this
                    //if (databaseContext.Matches == null)
                    //    Console.WriteLine("Pizdec");

                    databaseContext.Matches.Add(infoEntry);
                    databaseContext.SaveChanges();

                    statisticsUpdater.Update(infoEntry, databaseContext);
                    databaseContext.SaveChanges();
                }
            });
        }

        // todo fix keys(make composit key)
        public async Task<MatchInfo> GetMatchInfo(string endpoint, DateTime timestamp)
        {
            return await Task.Run(() =>
            {
                var entry = databaseContext.Matches.Find(endpoint + timestamp.ToString(CultureInfo.InvariantCulture));
                if (entry == null)
                    throw new NotFoundException("Entry not found");
                return new MatchInfo(entry);
            });
        }

        public async Task<List<ServersInfo>> GetServersInfo()
        {
            return await Task.Run(
                () => databaseContext.Servers
                    .ToList()
                    .Select(x => new ServersInfo(x))
                    .ToList());
        }

        public async Task<ServerStatistics> GetServerStatistics(string endpoint)
        {
            return await Task.Run(() =>
            {
                var entry = databaseContext.ServerStatistics.Find(endpoint);
                if (entry == null)
                    throw new NotFoundException("Entry not found");
                return new ServerStatistics(entry);
            });
        }

        public async Task<PlayerStatistics> GetPlayerStatistics(string name)
        {
            return await Task.Run(() =>
            {
                var entry = databaseContext.PlayersStatistics.Find(name);
                if (entry == null)
                    throw new NotFoundException("Entry not found");
                return new PlayerStatistics(entry);
            });
        }

        public async Task<List<RecentMatch>> GetRecentMatches(int count)
        {
            return await Task.Run(
                () => databaseContext.RecentMatches
                .OrderByDescending(x => x.Timestamp)
                .Take(count)
                .ToList()
                .Select(x => new RecentMatch(x))
                .ToList());
        }

        public async Task<List<BestPlayer>> GetBestPlayers(int count)
        {
            return await Task.Run(
                () => databaseContext.BestPlayers
                .OrderByDescending(x => x.KillToDeathRatio)
                .Take(count)
                .ToList()
                .Select(x => new BestPlayer(x))
                .ToList());
        }

        public async Task<List<PopularServer>> GetPopularServers(int count)
        {
            return await Task.Run(
                () => databaseContext.PopularServers
                .OrderByDescending(x => x.AverageMatchesPerDay)
                .Take(count)
                .ToList()
                .Select(x => new PopularServer(x))
                .ToList());
        }
    }
}
