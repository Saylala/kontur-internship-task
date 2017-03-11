using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Kontur.GameStats.Server.Database
{
    public class DatabaseInitializer
    {
        private readonly List<Action<SQLiteConnection>> initializers;

        public DatabaseInitializer()
        {
            initializers = new List<Action<SQLiteConnection>>
            {
                InitializeServersTable,
                InitializeMatchesTable,
                InitializeScoresTable,
                InitializeServersStatisticsTable,
                InitializePlayersStatisticsTable,

                InitializeRecentMatchesTable,
                InitializeBestPlayersTable,
                InitializePopularServersTable,

                InitializeStringEntriesTable,
                InitializeDayCountEntriesTable,
                InitializeNameCountEntriesTable,
                InitializeMatchCountEntriesTable
            };
        }

        public void InitializeDatabase(DatabaseContext context)
        {
            using (var connection = new SQLiteConnection(context.Database.Connection.ConnectionString))
            {
                connection.Open();
                foreach (var initializer in initializers)
                    initializer(connection);
            }
        }

        private void InitializeServersTable(SQLiteConnection connection)
        {
            new SQLiteCommand(
                    "CREATE TABLE IF NOT EXISTS Servers (" +
                    "Endpoint TEXT PRIMARY KEY," +
                    "Name TEXT)",
                    connection)
                .ExecuteNonQuery();
        }

        private void InitializeMatchesTable(SQLiteConnection connection)
        {
            new SQLiteCommand(
                    "CREATE TABLE IF NOT EXISTS Matches (" +
                    "Key TEXT PRIMARY KEY," +
                    "Endpoint TEXT," +
                    "Timestamp TEXT," +
                    "Map TEXT," +
                    "GameMode TEXT," +
                    "FragLimit INTEGER," +
                    "TimeLimit INTEGER," +
                    "TimeElapsed REAL)",
                    connection)
                .ExecuteNonQuery();
        }

        private void InitializeScoresTable(SQLiteConnection connection)
        {
            new SQLiteCommand(
                    "CREATE TABLE IF NOT EXISTS Scores (" +
                    "Id INTEGER PRIMARY KEY AUTOINCREMENT," +
                    "Name TEXT," +
                    "Frags INTEGER," +
                    "Kills INTEGER," +
                    "Deaths INTEGER," +
                    "MatchInfo_Key TEXT," +
                    "FOREIGN KEY (MatchInfo_Key) REFERENCES Matches(MatchInfo_Key))",
                    connection)
                .ExecuteNonQuery();
        }

        private void InitializeServersStatisticsTable(SQLiteConnection connection)
        {
            new SQLiteCommand(
                    "CREATE TABLE IF NOT EXISTS ServersStatistics (" +
                    "Endpoint TEXT PRIMARY KEY," +
                    "TotalMatchesPlayed INTEGER," +
                    "MaximumMatchesPerDay INTEGER," +
                    "AverageMatchesPerDay REAL," +
                    "MaximumPopulation INTEGER," +
                    "AveragePopulation REAL)",
                    connection)
                .ExecuteNonQuery();
        }

        private void InitializePlayersStatisticsTable(SQLiteConnection connection)
        {
            new SQLiteCommand(
                    "CREATE TABLE IF NOT EXISTS PlayersStatistics (" +
                    "Name TEXT PRIMARY KEY," +
                    "TotalMatchesPlayed INTEGER," +
                    "TotalMatchesWon INTEGER," +
                    "FavoriteServer TEXT," +
                    "UniqueServers INTEGER," +
                    "FavoriteGameMode TEXT," +
                    "AverageScoreboardPercent REAL," +
                    "MaximumMatchesPerDay INTEGER," +
                    "AverageMatchesPerDay REAL," +
                    "LastMatchPlayed TEXT," +
                    "KillToDeathRatio REAL," +
                    "TotalKills INTEGER," +
                    "TotalDeaths INTEGER)",
                    connection)
                .ExecuteNonQuery();
        }

        private void InitializeRecentMatchesTable(SQLiteConnection connection)
        {
            new SQLiteCommand(
                    "CREATE TABLE IF NOT EXISTS RecentMatches (" +
                    "Key TEXT PRIMARY KEY," +
                    "Server TEXT," +
                    "Timestamp TEXT," +
                    "FOREIGN KEY (Key) REFERENCES Matches(Key))",
                    connection)
                .ExecuteNonQuery();
        }

        private void InitializeBestPlayersTable(SQLiteConnection connection)
        {
            new SQLiteCommand(
                    "CREATE TABLE IF NOT EXISTS BestPlayers (" +
                    "Id INTEGER PRIMARY KEY," +
                    "Name TEXT," +
                    "KillToDeathRatio REAL)",
                    connection)
                .ExecuteNonQuery();
        }

        private void InitializePopularServersTable(SQLiteConnection connection)
        {
            new SQLiteCommand(
                    "CREATE TABLE IF NOT EXISTS PopularServers (" +
                    "Endpoint TEXT PRIMARY KEY," +
                    "Name TEXT," +
                    "AverageMatchesPerDay REAL)",
                    connection)
                .ExecuteNonQuery();
        }

        private void InitializeStringEntriesTable(SQLiteConnection connection)
        {
            new SQLiteCommand(
                    "CREATE TABLE IF NOT EXISTS StringEntries (" +
                    "Id INTEGER PRIMARY KEY AUTOINCREMENT," +
                    "String TEXT," +
                    "ServerInfo_Endpoint TEXT," +
                    "ServerStatistics_Endpoint TEXT," +
                    "ServerStatistics_Endpoint1 TEXT," +
                    "FOREIGN KEY (ServerInfo_Endpoint) REFERENCES Servers(ServerInfo_Endpoint)," +
                    "FOREIGN KEY (ServerStatistics_Endpoint) REFERENCES ServersStatistics(ServerStatistics_Endpoint)," +
                    "FOREIGN KEY (ServerStatistics_Endpoint1) REFERENCES ServersStatistics(ServerStatistics_Endpoint1))",
                    connection)
                .ExecuteNonQuery();
        }

        private void InitializeDayCountEntriesTable(SQLiteConnection connection)
        {
            new SQLiteCommand(
                    "CREATE TABLE IF NOT EXISTS DayCountEntries (" +
                    "Id INTEGER PRIMARY KEY AUTOINCREMENT," +
                    "Day TEXT," +
                    "Count INTEGER," +
                    "PlayerStatistics_Name TEXT," +
                    "ServerStatistics_Endpoint TEXT," +
                    "FOREIGN KEY (PlayerStatistics_Name) REFERENCES PlayersStatistics(PlayerStatistics_Name)," +
                    "FOREIGN KEY (ServerStatistics_Endpoint) REFERENCES ServersStatistics(ServerStatistics_Endpoint))",
                    connection)
                .ExecuteNonQuery();
        }

        private void InitializeNameCountEntriesTable(SQLiteConnection connection)
        {
            new SQLiteCommand(
                    "CREATE TABLE IF NOT EXISTS NameCountEntries (" +
                    "Id INTEGER PRIMARY KEY AUTOINCREMENT," +
                    "Name TEXT," +
                    "Count INTEGER," +
                    "PlayerStatistics_Name TEXT," +
                    "PlayerStatistics_Name1 TEXT," +
                    "ServerStatistics_Endpoint TEXT," +
                    "ServerStatistics_Endpoint1 TEXT," +
                    "FOREIGN KEY (PlayerStatistics_Name) REFERENCES PlayersStatistics(PlayerStatistics_Name)," +
                    "FOREIGN KEY (PlayerStatistics_Name1) REFERENCES PlayersStatistics(PlayerStatistics_Name1)," +
                    "FOREIGN KEY (ServerStatistics_Endpoint) REFERENCES ServersStatistics(ServerStatistics_Endpoint)," +
                    "FOREIGN KEY (ServerStatistics_Endpoint1) REFERENCES ServersStatistics(ServerStatistics_Endpoint1))",
                    connection)
                .ExecuteNonQuery();
        }

        private void InitializeMatchCountEntriesTable(SQLiteConnection connection)
        {
            new SQLiteCommand(
                    "CREATE TABLE IF NOT EXISTS MatchCountEntries (" +
                    "Key TEXT," +
                    "Endpoint TEXT," +
                    "TimeStamp TEXT," +
                    "Count INTEGER)",
                    //"PlayerStatistics_Name1 TEXT," +
                    //"ServerStatistics_Endpoint TEXT," +
                    //"ServerStatistics_Endpoint1 TEXT," +
                    //"FOREIGN KEY (PlayerStatistics_Name) REFERENCES PlayersStatistics(PlayerStatistics_Name)," +
                    //"FOREIGN KEY (PlayerStatistics_Name1) REFERENCES PlayersStatistics(PlayerStatistics_Name1)," +
                    //"FOREIGN KEY (ServerStatistics_Endpoint) REFERENCES ServersStatistics(ServerStatistics_Endpoint)," +
                    //"FOREIGN KEY (ServerStatistics_Endpoint1) REFERENCES ServersStatistics(ServerStatistics_Endpoint1))",
                    connection)
                .ExecuteNonQuery();
        }
    }
}
