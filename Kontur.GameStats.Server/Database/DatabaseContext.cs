using System.Data.Entity;
using Kontur.GameStats.Server.Models;
using SQLite.CodeFirst;

namespace Kontur.GameStats.Server.Database
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext() : base("DefaultConnection")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var model = modelBuilder.Build(Database.Connection);
            IDatabaseCreator sqliteDatabaseCreator = new SqliteDatabaseCreator();
            sqliteDatabaseCreator.Create(Database, model);
        }

        public DbSet<ServerInfo> Servers { get; set; }
        public DbSet<MatchInfo> Matches { get; set; }
        public DbSet<ServerStatistics> ServerStatistics { get; set; }
        public DbSet<PlayerStatistics> PlayersStatistics { get; set; }
        public DbSet<RecentMatch> RecentMatches { get; set; }
        public DbSet<BestPlayer> BestPlayers { get; set; }
        public DbSet<PopularServer> PopularServers { get; set; }
    }
}
