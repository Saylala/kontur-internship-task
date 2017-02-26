using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Globalization;

namespace Kontur.GameStats.Server.Database
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext() : base("DefaultConnection")
        {
        }

        public DbSet<ServerInfo> Servers { get; set; }
        public DbSet<MatchInfo> Matches { get; set; }
        public DbSet<ServerStatistics> ServerStatistics { get; set; }
        public DbSet<PlayerStatistics> PlayersStatistics { get; set; }
        public DbSet<RecentMatch> RecentMatches { get; set; }
        public DbSet<BestPlayer> BestPlayers { get; set; }
        public DbSet<PopularServer> PopularServers { get; set; }

        private readonly StatisticsUpdater statisticsUpdater = new StatisticsUpdater();

        public void PutServerInfo(string endpoint, ServerInfo info)
        {
            info.Endpoint = endpoint;
            Set<ServerInfo>().AddOrUpdate(info);
            SaveChanges();
        }

        public ServerInfo GetServerInfo(string endpoint)
        {
            return Servers.Find(endpoint);
        }

        public void PutMatchInfo(string endpoint, DateTime timestamp, MatchInfo info)
        {
            info.Key = endpoint + timestamp.ToString(CultureInfo.InvariantCulture);
            info.Endpoint = endpoint;
            info.Timestamp = timestamp;
            Matches.Add(info);
            statisticsUpdater.Update(info, this);
            SaveChanges();
        }

        public MatchInfo GetMatchInfo(string endpoint, DateTime timestamp)
        {
            return Matches.Find(endpoint, timestamp);
        }

        public IEnumerable<ServerInfo> GetServersInfo()
        {
            return Servers;
        }

        public ServerStatistics GetServerStatistics(string endpoint)
        {
            return ServerStatistics.Find(endpoint);
        }

        public PlayerStatistics GetPlayerStatistics(string name)
        {
            return PlayersStatistics.Find(name);
        }

        public IEnumerable<RecentMatch> GetRecentMatches()
        {
            return RecentMatches;
        }

        public IEnumerable<BestPlayer> GetBestPlayers()
        {
            return BestPlayers;
        }

        public IEnumerable<PopularServer> GetPopularServers()
        {
            return PopularServers;
        }
    }
}
