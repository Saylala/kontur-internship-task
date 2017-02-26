using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using FluentAssertions;
using Kontur.GameStats.Server.Database;
using Kontur.GameStats.Server.Models;
using NUnit.Framework;

namespace Kontur.GameStats.Server
{
    public class DatabaseTests
    {
        private DatabaseContext databaseContext;
        [SetUp]
        public void SetUp()
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", Directory.GetCurrentDirectory());
            databaseContext = new DatabaseContext();
        }

        [TearDown]
        public void TearDown()
        {
            databaseContext.Database.Delete();
            databaseContext.Dispose();
        }

        [Test]
        public void PutServerInfo_SavesInfo()
        {
            var data = new ServerInfo
            {
                Endpoint = "1234",
                Name = "Test",
                GameModes = new List<StringEntry> { new StringEntry { String = "DM" }, new StringEntry { String = "TDM" } }
            };

            using (var connection = new DatabaseContext())
                connection.PutServerInfo(data.Endpoint, new ServerInfo { Name = data.Name, GameModes = data.GameModes });
            var result = databaseContext.Servers.Find(data.Endpoint);

            result.ShouldBeEquivalentTo(data);
        }

        [Test]
        public void PutMatchInfo_SavesInfo()
        {
            var time = DateTime.Now;
            var matchData = new MatchInfo
            {
                Endpoint = "1234",
                Timestamp = time,
                Map = "DM-HelloWorld",
                GameMode = "DM",
                FragLimit = 20,
                TimeLimit = 20,
                TimeElapsed = 12.345678,
                Scoreboard = new List<Score>
                {
                    new Score { Name = "Player1", Frags = 20, Kills = 21, Deaths = 3 },
                    new Score { Name = "Player2", Frags = 2, Kills = 2, Deaths = 21 }
                }
            };
            var serverData = new ServerInfo
            {
                Endpoint = "1234",
                Name = "Test",
                GameModes = new List<StringEntry> { new StringEntry { String = "DM" }, new StringEntry { String = "TDM" } }
            };
            using (var connection = new DatabaseContext())
                connection.PutServerInfo(serverData.Endpoint, new ServerInfo { Name = serverData.Name, GameModes = serverData.GameModes });


            using (var connection = new DatabaseContext())
                connection.PutMatchInfo(matchData.Endpoint, matchData.Timestamp, new MatchInfo
                {
                    Map = matchData.Map,
                    GameMode = matchData.GameMode,
                    FragLimit = matchData.FragLimit,
                    TimeLimit = matchData.TimeLimit,
                    TimeElapsed = matchData.TimeElapsed,
                    Scoreboard = matchData.Scoreboard
                });
            var result = databaseContext.Matches.Find(matchData.Endpoint + time.ToString(CultureInfo.InvariantCulture));
            result.ShouldBeEquivalentTo(matchData, o => 
            {
                o.Excluding(x => x.Key);
                o.Excluding(x => x.Timestamp);
                return o;
            });
        }

        [Test]
        public void GetServersInfo_ReturnsCorrectInfo()
        {
            var data = new[]
            {
                new ServerInfo
                {
                    Endpoint = "1234",
                    Name = "Test",
                    GameModes = new List<StringEntry> {new StringEntry {String = "DM"}, new StringEntry {String = "TDM"}}
                },
                new ServerInfo
                {
                    Endpoint = "12345",
                    Name = "Another",
                    GameModes = new List<StringEntry> {new StringEntry {String = "TESTDM"}}
                }
            };

            using (var connection = new DatabaseContext())
                foreach (var entry in data)
                    connection.PutServerInfo(entry.Endpoint, new ServerInfo { Name = entry.Name, GameModes = entry.GameModes });

            var result = data.Select(entry => databaseContext.Servers.Find(entry.Endpoint)).ToList();

            for (var i = 0; i < data.Length; i++)
                result[i].ShouldBeEquivalentTo(data[i], o => o.Excluding(x => x.SelectedMemberPath.EndsWith("Id")));
        }

        [Test]
        public void GetServerStatistics_ReturnsCorrectStatistics()
        {
            var data = new ServerInfo
            {
                Endpoint = "1234",
                Name = "Test",
                GameModes = new List<StringEntry> { new StringEntry { String = "DM" }, new StringEntry { String = "TDM" } }
            };

            using (var connection = new DatabaseContext())
            {
                connection.PutServerInfo(data.Endpoint, new ServerInfo { Name = data.Name, GameModes = data.GameModes });
                foreach (var match in GenerateMatches(50))
                    connection.PutMatchInfo(data.Endpoint, match.Timestamp, match);
            }

            var result = databaseContext.GetServerStatistics(data.Endpoint);

            var expected = new ServerStatistics
            {
                AverageMatchesPerDay = 3.5714285714285716,
                AveragePopulation = 1.98,
                Endpoint = "1234",
                MaximumMatchesPerDay = 4,
                MaximumPopulation = 50,
                Top5GameModes = new List<StringEntry>
                {
                    new StringEntry {String = "0"},
                    new StringEntry {String = "1"},
                    new StringEntry {String = "2"},
                    new StringEntry {String = "3"},
                    new StringEntry {String = "4"}
                },
                Top5Maps = new List<StringEntry>
                {
                    new StringEntry {String = "0"},
                    new StringEntry {String = "1"},
                    new StringEntry {String = "2"},
                    new StringEntry {String = "3"},
                    new StringEntry {String = "4"}
                },
                TotalMatchesPlayed = 50
            };
            result.ShouldBeEquivalentTo(expected, o =>
            {
                o.Excluding(x => x.MatchesPerDay);
                o.Excluding(x => x.GameModePopularity);
                o.Excluding(x => x.MapPopularity);
                o.Excluding(x => x.PopulationPerMatch);
                o.Excluding(x => x.SelectedMemberPath.EndsWith("Id"));
                return o;
            });
        }

        private IEnumerable<MatchInfo> GenerateMatches(int count)
        {
            var matches = new List<MatchInfo>(count);
            for (var i = 0; i < count; i++)
            {
                var match = new MatchInfo();
                match.Timestamp = DateTime.Today.Add(new TimeSpan(i % 14, i % 24, i, i));
                match.Map = (i % 14).ToString();
                match.GameMode = (i % 15).ToString();
                match.FragLimit = i;
                match.TimeLimit = i;
                match.TimeElapsed = i;
                match.Scoreboard = GenerateScores(count);

                matches.Add(match);
            }
            return matches;
        }

        private List<Score> GenerateScores(int count)
        {
            var scores = new List<Score>();
            for (var i = 0; i < count; i++)
            {
                var score = new Score();
                score.Name = i.ToString();
                score.Deaths = i;
                score.Frags = count - i;
                score.Kills = count - i;

                scores.Add(score);
            }
            return scores;
        }
    }
}
