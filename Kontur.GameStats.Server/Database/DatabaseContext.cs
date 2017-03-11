using System.Data.Entity;
using Kontur.GameStats.Server.Models;

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
        public DbSet<StringEntry> StringEntries { get; set; }
        public DbSet<Score> Scores { get; set; }
        public DbSet<DayCountEntry> DayCountEntries { get; set; }
        public DbSet<MatchCountEntry> MatchCountEntries { get; set; }
        public DbSet<NameCountEntry> NameCountEntries { get; set; }
    }
}
