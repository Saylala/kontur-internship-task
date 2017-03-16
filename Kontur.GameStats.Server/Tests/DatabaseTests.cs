using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using FluentAssertions;
using Kontur.GameStats.Server.Database;
using Kontur.GameStats.Server.Models.DatabaseEntries;
using NUnit.Framework;

namespace Kontur.GameStats.Server.Tests
{
    public class DatabaseTests
    {
        private GameStatistics statistics;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", Directory.GetCurrentDirectory());
            statistics = new GameStatistics();
        }

        [Test]
        public void PutServerInfo_SavesInfo()
        {
            Console.WriteLine(Directory.GetCurrentDirectory());

            var data = new ServerInfoEntry
            {
                Endpoint = "PutServerInfo_SavesInfo",
                Name = "Test",
                GameModes = new List<StringEntry> { new StringEntry { String = "DM" }, new StringEntry { String = "TDM" } }
            };


            statistics.PutServerInfo(data.Endpoint, new ServerInfoEntry { Name = data.Name, GameModes = data.GameModes });
            ServerInfoEntry result;
            using (var databaseContext = new DatabaseContext())
            {
                result = databaseContext.Servers.Find(data.Endpoint);

                result.ShouldBeEquivalentTo(data, o => o.Excluding(x => x.SelectedMemberPath.EndsWith("Id")));
            }
        }

        [Test]
        public void PutMatchInfo_SavesInfo()
        {
            var time = DateTime.Now;
            var serverData = new ServerInfoEntry
            {
                Endpoint = "PutMatchInfo_SavesInfo",
                Name = "Test",
                GameModes = new List<StringEntry> { new StringEntry { String = "DM" }, new StringEntry { String = "TDM" } }
            };
            var matchData = new MatchInfoEntry
            {
                Endpoint = "PutMatchInfo_SavesInfo",
                Timestamp = time,
                Map = "DM-HelloWorld",
                GameMode = "DM",
                FragLimit = 20,
                TimeLimit = 20,
                TimeElapsed = 12.345678,
                Scoreboard = new List<ScoreEntry>
                {
                    new ScoreEntry {Name = "Player1", Frags = 20, Kills = 21, Deaths = 3},
                    new ScoreEntry {Name = "Player2", Frags = 2, Kills = 2, Deaths = 21}
                }
            };


            statistics.PutServerInfo(serverData.Endpoint, new ServerInfoEntry { Name = serverData.Name, GameModes = serverData.GameModes });
            statistics.PutMatchInfo(matchData.Endpoint, matchData.Timestamp, new MatchInfoEntry
            {
                Map = matchData.Map,
                GameMode = matchData.GameMode,
                FragLimit = matchData.FragLimit,
                TimeLimit = matchData.TimeLimit,
                TimeElapsed = matchData.TimeElapsed,
                Scoreboard = matchData.Scoreboard
            });
            MatchInfoEntry result;
            using (var databaseContext = new DatabaseContext())
            {
                result = databaseContext.Matches.Find(matchData.Endpoint + time.ToString(CultureInfo.InvariantCulture));


                result.ShouldBeEquivalentTo(matchData, o =>
                {
                    o.Excluding(x => x.Key);
                    o.Excluding(x => x.Timestamp);
                    o.Excluding(x => x.RecentMatchEntry);
                    return o;
                });
            }
        }

        [Test]
        public void GetServersInfo_ReturnsCorrectInfo()
        {
            var data = new[]
            {
                new ServerInfoEntry
                {
                    Endpoint = "GetServersInfo_ReturnsCorrectInfo1",
                    Name = "Test",
                    GameModes =
                        new List<StringEntry> {new StringEntry {String = "DM"}, new StringEntry {String = "TDM"}}
                },
                new ServerInfoEntry
                {
                    Endpoint = "GetServersInfo_ReturnsCorrectInfo2",
                    Name = "Another",
                    GameModes = new List<StringEntry> {new StringEntry {String = "TESTDM"}}
                }
            };

            foreach (var entry in data)
                statistics.PutServerInfo(entry.Endpoint, new ServerInfoEntry {Name = entry.Name, GameModes = entry.GameModes});
            var result = new GameStatistics().GetServersInfo();

            for (var i = 0; i < data.Length; i++)
                result[i].ShouldBeEquivalentTo(data[i], o => o.Excluding(x => x.SelectedMemberPath.EndsWith("Id")));
        }

        [Test]
        public void GetServerStatistics_ReturnsCorrectStatistics()
        {
            var data = new ServerInfoEntry
            {
                Endpoint = "GetServerStatistics_ReturnsCorrectStatistics",
                Name = "Test",
                GameModes = new List<StringEntry> { new StringEntry { String = "DM" }, new StringEntry { String = "TDM" } }
            };
            var expected = new ServerStatisticsEntry
            {
                AverageMatchesPerDay = 3.5714285714285716,
                AveragePopulation = 1.98,
                Endpoint = "GetServerStatistics_ReturnsCorrectStatistics",
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


            statistics.PutServerInfo(data.Endpoint, new ServerInfoEntry { Name = data.Name, GameModes = data.GameModes });

            var sw = Stopwatch.StartNew();
            foreach (var match in GenerateMatches(50, "GetServerStatistics_ReturnsCorrectStatistics"))
                statistics.PutMatchInfo(data.Endpoint, match.Timestamp, match);
            Console.WriteLine(sw.ElapsedMilliseconds);

            var result = new GameStatistics().GetServerStatistics(data.Endpoint);


            result.ShouldBeEquivalentTo(expected, o =>
            {
                o.Excluding(x => x.Top5GameModes);
                o.Excluding(x => x.Top5Maps);

                o.Excluding(x => x.SelectedMemberPath.EndsWith("Id"));
                return o;
            });
        }

        [Test]
        public void GetPlayerStatistics_ReturnsCorrectStatistics()
        {
            var data = new ServerInfoEntry
            {
                Endpoint = "GetPlayerStatistics_ReturnsCorrectStatistics",
                Name = "Test",
                GameModes = new List<StringEntry> { new StringEntry { String = "DM" }, new StringEntry { String = "TDM" } }
            };
            var expected = new PlayerStatisticsEntry
            {
                TotalMatchesPlayed = 50,
                TotalMatchesWon = 50,
                FavoriteServer = "GetPlayerStatistics_ReturnsCorrectStatistics",
                UniqueServers = 1,
                FavoriteGameMode = "0",
                AverageScoreboardPercent = 100,
                MaximumMatchesPerDay = 4,
                AverageMatchesPerDay = 3.5714285714285716,
                KillToDeathRatio = 0
            };


            statistics.PutServerInfo(data.Endpoint, new ServerInfoEntry { Name = data.Name, GameModes = data.GameModes });
            foreach (var match in GenerateMatches(50, "GetPlayerStatistics_ReturnsCorrectStatistics"))
                statistics.PutMatchInfo(data.Endpoint, match.Timestamp, match);
            var result = new GameStatistics().GetPlayerStatistics("GetPlayerStatistics_ReturnsCorrectStatistics0");


            result.ShouldBeEquivalentTo(expected, o =>
            {
                o.Excluding(x => x.LastMatchPlayed);
                return o;
            });
        }

        [TestCase(1)]
        [TestCase(15)]
        //[TestCase(50)]
        //[TestCase(100)]
        public void GetRecentMatches_ReturnsCorrectStatistics(int count)
        {
            statistics = new GameStatistics();

            var data = new ServerInfoEntry
            {
                Endpoint = "GetRecentMatches_ReturnsCorrectStatistics" + count,
                Name = "Test",
                GameModes = new List<StringEntry> { new StringEntry { String = "DM" }, new StringEntry { String = "TDM" } }
            };
            var matches = GenerateMatches(count, "GetRecentMatches_ReturnsCorrectStatistics" + count).ToList();
            var expected = matches
                .OrderByDescending(x => x.Timestamp)
                .Select(x => new RecentMatchEntry { Timestamp = x.Timestamp, Server = data.Endpoint, MatchInfoEntry = x })
                .Take(count)
                .ToList();


            statistics.PutServerInfo(data.Endpoint, new ServerInfoEntry { Name = data.Name, GameModes = data.GameModes });

            var sw = Stopwatch.StartNew();

            foreach (var match in matches)
                statistics.PutMatchInfo(data.Endpoint, match.Timestamp, match);

            Console.WriteLine(sw.ElapsedMilliseconds);

            var result = new GameStatistics().GetRecentMatches(count).ToList();


            result.Count.ShouldBeEquivalentTo(expected.Count);
            for (var i = 0; i < result.Count; i++)
                result[i].ShouldBeEquivalentTo(expected[i]);
        }

        [Test]
        public void GetBestPlayers_DoesNotCountPlayersWithoutDeaths()
        {
            var time = DateTime.Now;
            var data = new ServerInfoEntry
            {
                Endpoint = "GetBestPlayers_DoesNotCountPlayersWithoutDeaths",
                Name = "Test",
                GameModes = new List<StringEntry> { new StringEntry { String = "DM" }, new StringEntry { String = "TDM" } }
            };
            var match = new MatchInfoEntry
            {
                Map = "1",
                GameMode = "2",
                FragLimit = 20,
                TimeLimit = 300,
                TimeElapsed = 25,
                Scoreboard = new List<ScoreEntry>
                {
                    new ScoreEntry { Name = "GetBestPlayers_DoesNotCountPlayersWithoutDeaths1", Deaths = 0, Frags = 2, Kills = 20 },
                    new ScoreEntry { Name = "GetBestPlayers_DoesNotCountPlayersWithoutDeaths2", Deaths = 0, Frags = 20, Kills = 2}
                }
            };


            statistics.PutServerInfo(data.Endpoint, new ServerInfoEntry { Name = data.Name, GameModes = data.GameModes });
            statistics.PutMatchInfo(data.Endpoint, time, match);


            new GameStatistics().GetBestPlayers(50).Should().BeEmpty();
        }

        [Test]
        public void GetBestPlayers_DoesNotCountPlayersWithLessThen10Games()
        {
            var date = DateTime.Now.Date;
            var data = new ServerInfoEntry
            {
                Endpoint = "GetBestPlayers_DoesNotCountPlayersWithLessThen10Games",
                Name = "Test",
                GameModes = new List<StringEntry> { new StringEntry { String = "DM" }, new StringEntry { String = "TDM" } }
            };
            var match = new MatchInfoEntry
            {
                Map = "1",
                GameMode = "2",
                FragLimit = 20,
                TimeLimit = 300,
                TimeElapsed = 25,
                Scoreboard = new List<ScoreEntry>
                {
                    new ScoreEntry { Name = "GetBestPlayers_DoesNotCountPlayersWithLessThen10Games1", Deaths = 1, Frags = 2, Kills = 20 },
                    new ScoreEntry { Name = "GetBestPlayers_DoesNotCountPlayersWithLessThen10Games2", Deaths = 1, Frags = 20, Kills = 2}
                }
            };


            statistics.PutServerInfo(data.Endpoint, new ServerInfoEntry { Name = data.Name, GameModes = data.GameModes });
            for (var i = 0; i < 9; i++)
                new GameStatistics().PutMatchInfo(data.Endpoint, date + TimeSpan.FromHours(i), match);


            new GameStatistics().GetBestPlayers(50).Should().BeEmpty();
        }

        [Test]
        public void GetBestPlayers_ReturnsCorrectStatistics()
        {
            var date = DateTime.Now.Date;
            var data = new ServerInfoEntry
            {
                Endpoint = "GetBestPlayers_ReturnsCorrectStatistics",
                Name = "Test",
                GameModes = new List<StringEntry> { new StringEntry { String = "DM" }, new StringEntry { String = "TDM" } }
            };
            var match = new MatchInfoEntry
            {
                Map = "1",
                GameMode = "2",
                FragLimit = 20,
                TimeLimit = 300,
                TimeElapsed = 25,
                Scoreboard = new List<ScoreEntry>
                {
                    new ScoreEntry { Name = "GetBestPlayers_ReturnsCorrectStatistics1", Deaths = 1, Frags = 2, Kills = 20 },
                    new ScoreEntry { Name = "GetBestPlayers_ReturnsCorrectStatistics2", Deaths = 1, Frags = 20, Kills = 2}
                }
            };
            var expected = new List<BestPlayerEntry>
            {
                new BestPlayerEntry {Name = "GetBestPlayers_ReturnsCorrectStatistics1", KillToDeathRatio = 20},
                new BestPlayerEntry {Name = "GetBestPlayers_ReturnsCorrectStatistics2", KillToDeathRatio = 2}
            };


            statistics.PutServerInfo(data.Endpoint, new ServerInfoEntry { Name = data.Name, GameModes = data.GameModes });
            for (var i = 0; i < 20; i++)
                new GameStatistics().PutMatchInfo(data.Endpoint, date + TimeSpan.FromHours(i), match);
            var result = new GameStatistics().GetBestPlayers(2).ToList();


            result.Count.ShouldBeEquivalentTo(expected.Count);
            for (var i = 0; i < expected.Count; i++)
                result[i].ShouldBeEquivalentTo(expected[i]);
        }

        [Test]
        public void GetPopularServers_ReturnsCorrectStatistics()
        {
            var date = DateTime.Now.Date;
            var servers = new[]
            {
                new ServerInfoEntry
                {
                    Endpoint = "GetPopularServers_ReturnsCorrectStatistics1",
                    Name = "Server1",
                    GameModes =
                        new List<StringEntry> {new StringEntry { String = "DM"}, new StringEntry { String = "TDM"}}
                },
                new ServerInfoEntry
                {
                    Endpoint = "GetPopularServers_ReturnsCorrectStatistics2",
                    Name = "Server2",
                    GameModes =
                        new List<StringEntry> {new StringEntry { String = "DM"}, new StringEntry { String = "TDM"}}
                }
            };
            var match = new MatchInfoEntry
            {
                Map = "1",
                GameMode = "2",
                FragLimit = 20,
                TimeLimit = 300,
                TimeElapsed = 25,
                Scoreboard = new List<ScoreEntry>
                {
                    new ScoreEntry { Name = "GetPopularServers_ReturnsCorrectStatistics1", Deaths = 1, Frags = 2, Kills = 20 },
                    new ScoreEntry { Name = "GetPopularServers_ReturnsCorrectStatistics2", Deaths = 1, Frags = 20, Kills = 2}
                }
            };
            var expected = new List<PopularServerEntry>
            {
                new PopularServerEntry { Endpoint = "GetPopularServers_ReturnsCorrectStatistics1", Name = "Server1", AverageMatchesPerDay = 3},
                new PopularServerEntry {Endpoint = "GetPopularServers_ReturnsCorrectStatistics2", Name = "Server2", AverageMatchesPerDay = 2}
            };


            foreach (var server in servers)
                new GameStatistics().PutServerInfo(server.Endpoint, new ServerInfoEntry { Name = server.Name, GameModes = server.GameModes });
            for (var i = 0; i < 5; i++)
            {
                var server = servers[i % 2];
                new GameStatistics().PutMatchInfo(server.Endpoint, date + TimeSpan.FromHours(i), match);
            }
            var result = new GameStatistics().GetPopularServers(2).ToList();


            result.Count.ShouldBeEquivalentTo(expected.Count);
            for (var i = 0; i < expected.Count; i++)
                result[i].ShouldBeEquivalentTo(expected[i]);
        }

        private IEnumerable<MatchInfoEntry> GenerateMatches(int count, string name)
        {
            var matches = new List<MatchInfoEntry>(count);
            for (var i = 0; i < count; i++)
            {
                var match = new MatchInfoEntry
                {
                    Timestamp = DateTime.Today.Add(new TimeSpan(i % 14, i % 24, i, i)),
                    Map = (i % 14).ToString(),
                    GameMode = (i % 15).ToString(),
                    FragLimit = i,
                    TimeLimit = i,
                    TimeElapsed = i,
                    Scoreboard = GenerateScores(count, name)
                };

                matches.Add(match);
            }
            return matches;
        }

        private List<ScoreEntry> GenerateScores(int count, string name)
        {
            var scores = new List<ScoreEntry>();
            for (var i = 0; i < count; i++)
            {
                var score = new ScoreEntry
                {
                    Name = name + i,
                    Deaths = i,
                    Frags = count - i,
                    Kills = count - i
                };

                scores.Add(score);
            }
            return scores;
        }
    }
}
