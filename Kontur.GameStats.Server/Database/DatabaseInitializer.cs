using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Kontur.GameStats.Server.Database
{
    public class DatabaseInitializer
    {
        private static readonly List<Action<SQLiteConnection>> initializers = new List<Action<SQLiteConnection>>
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

        public static void InitializeDatabase(DatabaseContext context)
        {
            using (var connection = new SQLiteConnection(context.Database.Connection.ConnectionString))
            {
                connection.Open();

                foreach (var initializer in initializers)
                    initializer(connection);
            }
        }

        private static void InitializeServersTable(SQLiteConnection connection)
        {
            ExecuteCommand(
                "CREATE TABLE IF NOT EXISTS Servers (" +
                "Endpoint TEXT PRIMARY KEY," +
                "Name TEXT)",
                connection);
        }

        private static void InitializeMatchesTable(SQLiteConnection connection)
        {
            ExecuteCommand(
                "CREATE TABLE IF NOT EXISTS Matches (" +
                "Key TEXT PRIMARY KEY," +
                "Endpoint TEXT," +
                "Timestamp TEXT," +
                "Map TEXT," +
                "GameMode TEXT," +
                "FragLimit INTEGER," +
                "TimeLimit INTEGER," +
                "TimeElapsed REAL)",
                connection);
        }

        private static void InitializeScoresTable(SQLiteConnection connection)
        {
            ExecuteCommand(
                "CREATE TABLE IF NOT EXISTS Scores (" +
                "Id INTEGER PRIMARY KEY AUTOINCREMENT," +
                "Name TEXT," +
                "Frags INTEGER," +
                "Kills INTEGER," +
                "Deaths INTEGER," +
                "MatchInfoEntry_Key TEXT," +
                "FOREIGN KEY (MatchInfoEntry_Key) REFERENCES Matches(MatchInfoEntry_Key))",
                connection);
        }

        private static void InitializeServersStatisticsTable(SQLiteConnection connection)
        {
            ExecuteCommand(
                "CREATE TABLE IF NOT EXISTS ServersStatistics (" +
                "Endpoint TEXT PRIMARY KEY," +
                "TotalMatchesPlayed INTEGER," +
                "MaximumMatchesPerDay INTEGER," +
                "AverageMatchesPerDay REAL," +
                "MaximumPopulation INTEGER," +
                "AveragePopulation REAL)",
                connection);
        }

        private static void InitializePlayersStatisticsTable(SQLiteConnection connection)
        {
            ExecuteCommand(
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
                connection);
        }

        private static void InitializeRecentMatchesTable(SQLiteConnection connection)
        {
            ExecuteCommand(
                "CREATE TABLE IF NOT EXISTS RecentMatches (" +
                "Key TEXT PRIMARY KEY," +
                "Server TEXT," +
                "Timestamp TEXT," +
                "FOREIGN KEY (Key) REFERENCES Matches(Key))",
                connection);
        }

        private static void InitializeBestPlayersTable(SQLiteConnection connection)
        {
            ExecuteCommand(
                "CREATE TABLE IF NOT EXISTS BestPlayers (" +
                "Id INTEGER PRIMARY KEY," +
                "Name TEXT," +
                "KillToDeathRatio REAL)",
                connection);
        }

        private static void InitializePopularServersTable(SQLiteConnection connection)
        {
            ExecuteCommand(
                "CREATE TABLE IF NOT EXISTS PopularServers (" +
                "Endpoint TEXT PRIMARY KEY," +
                "Name TEXT," +
                "AverageMatchesPerDay REAL)",
                connection);
        }

        private static void InitializeStringEntriesTable(SQLiteConnection connection)
        {
            ExecuteCommand(
                "CREATE TABLE IF NOT EXISTS StringEntries (" +
                "Id INTEGER PRIMARY KEY AUTOINCREMENT," +
                "String TEXT," +
                "ServerInfoEntry_Endpoint TEXT," +
                "ServerStatisticsEntry_Endpoint TEXT," +
                "ServerStatisticsEntry_Endpoint1 TEXT," +
                "FOREIGN KEY (ServerInfoEntry_Endpoint) REFERENCES Servers(ServerInfoEntry_Endpoint)," +
                "FOREIGN KEY (ServerStatisticsEntry_Endpoint) REFERENCES ServersStatistics(ServerStatisticsEntry_Endpoint)," +
                "FOREIGN KEY (ServerStatisticsEntry_Endpoint1) REFERENCES ServersStatistics(ServerStatisticsEntry_Endpoint1))",
                connection);
        }

        private static void InitializeDayCountEntriesTable(SQLiteConnection connection)
        {
            ExecuteCommand(
                "CREATE TABLE IF NOT EXISTS DayCountEntries (" +
                "Id INTEGER PRIMARY KEY AUTOINCREMENT," +
                "Day TEXT," +
                "Count INTEGER," +
                "PlayerStatisticsEntry_Name TEXT," +
                "ServerStatisticsEntry_Endpoint TEXT," +
                "FOREIGN KEY (PlayerStatisticsEntry_Name) REFERENCES PlayersStatistics(PlayerStatisticsEntry_Name)," +
                "FOREIGN KEY (ServerStatisticsEntry_Endpoint) REFERENCES ServersStatistics(ServerStatisticsEntry_Endpoint))",
                connection);
        }

        private static void InitializeNameCountEntriesTable(SQLiteConnection connection)
        {
            ExecuteCommand(
                "CREATE TABLE IF NOT EXISTS NameCountEntries (" +
                "Id INTEGER PRIMARY KEY AUTOINCREMENT," +
                "Name TEXT," +
                "Count INTEGER," +
                "PlayerStatisticsEntry_Name TEXT," +
                "PlayerStatisticsEntry_Name1 TEXT," +
                "ServerStatisticsEntry_Endpoint TEXT," +
                "ServerStatisticsEntry_Endpoint1 TEXT," +
                "FOREIGN KEY (PlayerStatisticsEntry_Name) REFERENCES PlayersStatistics(PlayerStatisticsEntry_Name)," +
                "FOREIGN KEY (PlayerStatisticsEntry_Name1) REFERENCES PlayersStatistics(PlayerStatisticsEntry_Name1)," +
                "FOREIGN KEY (ServerStatisticsEntry_Endpoint) REFERENCES ServersStatistics(ServerStatisticsEntry_Endpoint)," +
                "FOREIGN KEY (ServerStatisticsEntry_Endpoint1) REFERENCES ServersStatistics(ServerStatisticsEntry_Endpoint1))",
                connection);
        }

        private static void InitializeMatchCountEntriesTable(SQLiteConnection connection)
        {
            ExecuteCommand(
                "CREATE TABLE IF NOT EXISTS MatchCountEntries (" +
                "Key TEXT," +
                "Endpoint TEXT," +
                "TimeStamp TEXT," +
                "Count INTEGER)",
                connection);
        }

        private static void ExecuteCommand(string command, SQLiteConnection connection)
        {
            using (var sqLiteCommand = new SQLiteCommand(command, connection))
                sqLiteCommand.ExecuteNonQuery();
        }
    }
}
