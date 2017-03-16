using System.Data.Entity;
using Kontur.GameStats.Server.Models.DatabaseEntries;

namespace Kontur.GameStats.Server.Database
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext() : base("DefaultConnection")
        {
        }

        public DbSet<ServerInfoEntry> Servers { get; set; }
        public DbSet<MatchInfoEntry> Matches { get; set; }
        public DbSet<ServerStatisticsEntry> ServerStatistics { get; set; }
        public DbSet<PlayerStatisticsEntry> PlayersStatistics { get; set; }
        public DbSet<RecentMatchEntry> RecentMatches { get; set; }
        public DbSet<BestPlayerEntry> BestPlayers { get; set; }
        public DbSet<PopularServerEntry> PopularServers { get; set; }

        public DbSet<StringEntry> StringEntries { get; set; }

        //public DbSet<Score> Scores { get; set; }
        //public DbSet<DayCountEntry> DayCountEntries { get; set; }
        //public DbSet<MatchCountEntry> MatchCountEntries { get; set; }
        //public DbSet<NameCountEntry> NameCountEntries { get; set; }
    }
}
