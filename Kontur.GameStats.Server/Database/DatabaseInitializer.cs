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
            ExecuteCommand(
                "CREATE TABLE IF NOT EXISTS Servers (" +
                "Endpoint TEXT PRIMARY KEY," +
                "Name TEXT)",
                connection);
        }

        private void InitializeMatchesTable(SQLiteConnection connection)
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

        private void InitializeScoresTable(SQLiteConnection connection)
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

        private void InitializeServersStatisticsTable(SQLiteConnection connection)
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

        private void InitializePlayersStatisticsTable(SQLiteConnection connection)
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

        private void InitializeRecentMatchesTable(SQLiteConnection connection)
        {
            ExecuteCommand(
                "CREATE TABLE IF NOT EXISTS RecentMatches (" +
                "Key TEXT PRIMARY KEY," +
                "Server TEXT," +
                "Timestamp TEXT," +
                "FOREIGN KEY (Key) REFERENCES Matches(Key))",
                connection);
        }

        private void InitializeBestPlayersTable(SQLiteConnection connection)
        {
            ExecuteCommand(
                "CREATE TABLE IF NOT EXISTS BestPlayers (" +
                "Id INTEGER PRIMARY KEY," +
                "Name TEXT," +
                "KillToDeathRatio REAL)",
                connection);
        }

        private void InitializePopularServersTable(SQLiteConnection connection)
        {
            ExecuteCommand(
                "CREATE TABLE IF NOT EXISTS PopularServers (" +
                "Endpoint TEXT PRIMARY KEY," +
                "Name TEXT," +
                "AverageMatchesPerDay REAL)",
                connection);
        }

        private void InitializeStringEntriesTable(SQLiteConnection connection)
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

        private void InitializeDayCountEntriesTable(SQLiteConnection connection)
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

        private void InitializeNameCountEntriesTable(SQLiteConnection connection)
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

        private void InitializeMatchCountEntriesTable(SQLiteConnection connection)
        {
            ExecuteCommand(
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
                connection);
        }

        private void ExecuteCommand(string command, SQLiteConnection connection)
        {
            using (var sqLiteCommand = new SQLiteCommand(command, connection))
                sqLiteCommand.ExecuteNonQuery();
        }
    }
}
