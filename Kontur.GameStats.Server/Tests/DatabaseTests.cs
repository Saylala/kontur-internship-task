using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Kontur.GameStats.Server.Database;
using Kontur.GameStats.Server.Exceptions;
using Kontur.GameStats.Server.Models.DatabaseEntries;
using Kontur.GameStats.Server.Models.Serialization;
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
        [Order(100)]
        public async Task PutServerInfo_SavesInfo()
        {
            Console.WriteLine(Directory.GetCurrentDirectory());
            var endpoint = "PutServerInfo_SavesInfo";
            var data = new ServerInfoEntry { Name = "Test", GameModes = new List<StringEntry> { new StringEntry { String = "DM" } } };
            await statistics.PutServerInfo(endpoint, data);
            using (var databaseContext = new DatabaseContext())
            {
                var result = databaseContext.Servers.Find(endpoint);
                result.ShouldBeEquivalentTo(data, o => o.Excluding(x => x.SelectedMemberPath.EndsWith("Id")));
            }
        }

        [Test]
        [Order(101)]
        public async Task PutServerInfo_ResavesInfo()
        {
            var endpoint = "PutServerInfo_SavesInfo";
            var newData = new ServerInfoEntry { Name = "NewName", GameModes = new List<StringEntry> { new StringEntry { String = "TM" }, new StringEntry { String = "TDM" } } };
            await statistics.PutServerInfo(endpoint, newData);
            using (var databaseContext = new DatabaseContext())
            {
                var result = databaseContext.Servers.Find(endpoint);
                result.ShouldBeEquivalentTo(newData, o => o.Excluding(x => x.SelectedMemberPath.EndsWith("Id")));
            }
        }

        [Test]
        [Order(201)]
        public async Task PutMatchInfo_SavesInfo()
        {
            var endpoint = "PutServerInfo_SavesInfo";
            var timeStamp = DateTime.UtcNow.Date - TimeSpan.FromDays(3);
            var matchData = new MatchInfoEntry
            {
                Map = "DM-HelloWorld",
                GameMode = "TM",
                FragLimit = 20,
                TimeLimit = 20,
                TimeElapsed = 12.345678,
                Scoreboard = new List<ScoreEntry>
                {
                    new ScoreEntry {Name = "Player1", Frags = 20, Kills = 21, Deaths = 3},
                    new ScoreEntry {Name = "Player2", Frags = 2, Kills = 2, Deaths = 21}
                }
            };

            await statistics.PutMatchInfo(endpoint, timeStamp, matchData);
            using (var databaseContext = new DatabaseContext())
            {
                var result = databaseContext.Matches.Find(matchData.Endpoint + timeStamp.ToString(CultureInfo.InvariantCulture));


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
        [Order(201)]
        public async Task PutMatchInfo_DoesNot_ResavesInfo()
        {
            var endpoint = "PutServerInfo_SavesInfo";
            var timeStamp = DateTime.UtcNow.Date - TimeSpan.FromDays(3);
            var matchData = new MatchInfoEntry
            {
                Map = "NewMap",
                GameMode = "TDM",
                FragLimit = 200,
                TimeLimit = 200,
                TimeElapsed = 152.9,
                Scoreboard = new List<ScoreEntry>
                {
                    new ScoreEntry {Name = "Player2", Frags = 20, Kills = 21, Deaths = 3},
                    new ScoreEntry {Name = "Player1", Frags = 2, Kills = 2, Deaths = 21}
                }
            };

            try
            {
                await statistics.PutMatchInfo(endpoint, timeStamp, matchData);
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is BadRequestException);
            }
        }

        [Test]
        [Order(300)]
        public async Task GetServerInfo_ThrowsNotFoundOnNonexistent()
        {
            var endpoint = "NonexistentEndpoint";

            try
            {
                await statistics.GetServerInfo(endpoint);
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is NotFoundException);
            }
        }

        [Test]
        [Order(301)]
        public async Task GetMatchInfo_ThrowsNotFoundOnNonexistent()
        {
            var endpoint = "PutServerInfo_SavesInfo";
            var timeStamp = DateTime.UtcNow.Date;

            try
            {
                await statistics.GetMatchInfo(endpoint, timeStamp);
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is NotFoundException);
            }
        }

        [Test]
        [Order(400)]
        public async Task GetServersInfo_ReturnsCorrectInfo()
        {
            var data = new[]
            {
                new ServerInfoEntry
                {
                    Endpoint = "PutServerInfo_SavesInfo",
                    Name = "NewName",
                    GameModes = new List<StringEntry> {new StringEntry {String = "TM"}, new StringEntry { String = "TDM" }}
                },
                new ServerInfoEntry
                {
                    Endpoint = "GetServersInfo_ReturnsCorrectInfo2",
                    Name = "Another",
                    GameModes = new List<StringEntry> {new StringEntry {String = "TESTDM"}}
                }
            };

            await statistics.PutServerInfo(data[1].Endpoint, data[1]);
            var result = statistics.GetServersInfo().Result;

            result.Count.ShouldBeEquivalentTo(data.Length);
            for (var i = 0; i < data.Length; i++)
            {
                result[i].Endpoint.ShouldBeEquivalentTo(data[i].Endpoint);
                result[i].Info.Name.ShouldBeEquivalentTo(data[i].Name);
                result[i].Info.GameModes.Count.ShouldBeEquivalentTo(data[i].GameModes.Count);
                for (int j = 0; j < result[i].Info.GameModes.Count; j++)
                    result[i].Info.GameModes[j].ShouldBeEquivalentTo(data[i].GameModes[j].String);
            }

        }

        [Test]
        [Order(500)]
        public async Task GetPlayerStatistics_ReturnsCorrectStatistics()
        {
            var servers = new[]
            {
                new ServerInfoEntry
                {
                    Endpoint = "Server1",
                    Name = "Test",
                    GameModes = new List<StringEntry> {new StringEntry {String = "DM"}, new StringEntry {String = "TDM"}}
                },
                new ServerInfoEntry
                {
                    Endpoint = "Server2",
                    Name = "Test",
                    GameModes = new List<StringEntry> {new StringEntry {String = "DM"}, new StringEntry {String = "TDM"}}
                }
            };
            var matches = new[]
            {
                new MatchInfoEntry
                {
                    Endpoint = "Server1",
                    Timestamp = DateTime.Today,
                    Map = "NewMap",
                    GameMode = "DM",
                    FragLimit = 200,
                    TimeLimit = 200,
                    TimeElapsed = 152.9,
                    Scoreboard = new List<ScoreEntry>
                    {
                        new ScoreEntry {Name = "Player1", Frags = 15, Kills = 6, Deaths = 3},
                        new ScoreEntry {Name = "Player2", Frags = 2, Kills = 2, Deaths = 21}
                    }
                },
                new MatchInfoEntry
                {
                    Endpoint = "Server2",
                    Timestamp = DateTime.UtcNow.Date + TimeSpan.FromDays(3),
                    Map = "NewMap",
                    GameMode = "TDM",
                    FragLimit = 200,
                    TimeLimit = 200,
                    TimeElapsed = 152.9,
                    Scoreboard = new List<ScoreEntry>
                    {
                        new ScoreEntry {Name = "Player3", Frags = 33, Kills = 2, Deaths = 3},
                        new ScoreEntry {Name = "Player1", Frags = 12, Kills = 6, Deaths = 21},
                        new ScoreEntry {Name = "Player5", Frags = 1, Kills = 3, Deaths = 21}
                    }
                },
                new MatchInfoEntry
                {
                    Endpoint = "PutServerInfo_SavesInfo",
                    Timestamp = DateTime.UtcNow.Date + TimeSpan.FromDays(3),
                    Map = "NewMap",
                    GameMode = "TDM",
                    FragLimit = 200,
                    TimeLimit = 200,
                    TimeElapsed = 152.9,
                    Scoreboard = new List<ScoreEntry>
                    {
                        new ScoreEntry {Name = "Player3", Frags = 33, Kills = 2, Deaths = 3},
                        new ScoreEntry {Name = "Player4", Frags = 12, Kills = 6, Deaths = 21},
                        new ScoreEntry {Name = "Player6", Frags = 12, Kills = 6, Deaths = 24},
                        new ScoreEntry {Name = "Player5", Frags = 1, Kills = 3, Deaths = 21}
                    }
                }
            };
            var expected = new PlayerStatisticsEntry
            {
                TotalMatchesPlayed = 3,
                TotalMatchesWon = 2,
                FavoriteServer = "PutServerInfo_SavesInfo",
                UniqueServers = 3,
                FavoriteGameMode = "TM",
                AverageScoreboardPercent = (100 + 100 + 50) / 3.0,
                MaximumMatchesPerDay = 1,
                AverageMatchesPerDay = 3 / 6.0,
                KillToDeathRatio = 33.0 / 27
            };

            foreach (var server in servers)
                await statistics.PutServerInfo(server.Endpoint, new ServerInfoEntry { Name = server.Name, GameModes = server.GameModes });
            foreach (var match in matches)
                await statistics.PutMatchInfo(match.Endpoint, match.Timestamp, match);
            var result = statistics.GetPlayerStatistics("Player1").Result;

            result.ShouldBeEquivalentTo(expected, o =>
            {
                o.Excluding(x => x.LastMatchPlayed);
                return o;
            });
        }

        [Test]
        [Order(600)]
        public async Task GetServerStatistics_ReturnsCorrectStatistics()
        {
            var endpoint = "PutServerInfo_SavesInfo";

            var expected = new ServerStatisticsEntry
            {
                TotalMatchesPlayed = 2,
                MaximumMatchesPerDay = 1,
                AverageMatchesPerDay = 2/6.0,
                MaximumPopulation = 4,
                AveragePopulation = 3,
                Top5GameModes = new List<StringEntry> { new StringEntry {String = "TM"}, new StringEntry {String = "TDM"} },
                Top5Maps = new List<StringEntry> { new StringEntry {String = "DM-HelloWorld" }, new StringEntry {String = "NewMap"} }
            };

            var result = await statistics.GetServerStatistics(endpoint);
            
            result.ShouldBeEquivalentTo(expected, o =>
            {
                o.Excluding(x => x.Top5GameModes);
                o.Excluding(x => x.Top5Maps);
                o.Excluding(x => x.SelectedMemberPath.EndsWith("Id"));
                return o;
            });
        }

        [Test]
        [Order(700)]
        public async Task GetRecentMatches_ReturnsCorrectStatistics()
        {
            var expected = new[]
            {
                new MatchInfoEntry
                {
                    Endpoint = "Server2",
                    Timestamp = DateTime.UtcNow.Date + TimeSpan.FromDays(3),
                    Map = "NewMap",
                    GameMode = "TDM",
                    FragLimit = 200,
                    TimeLimit = 200,
                    TimeElapsed = 152.9,
                    Scoreboard = new List<ScoreEntry>
                    {
                        new ScoreEntry {Name = "Player3", Frags = 33, Kills = 2, Deaths = 3},
                        new ScoreEntry {Name = "Player1", Frags = 12, Kills = 6, Deaths = 21},
                        new ScoreEntry {Name = "Player5", Frags = 1, Kills = 3, Deaths = 21}
                    }
                },
                new MatchInfoEntry
                {
                    Endpoint = "PutServerInfo_SavesInfo",
                    Timestamp = DateTime.UtcNow.Date + TimeSpan.FromDays(3),
                    Map = "NewMap",
                    GameMode = "TDM",
                    FragLimit = 200,
                    TimeLimit = 200,
                    TimeElapsed = 152.9,
                    Scoreboard = new List<ScoreEntry>
                    {
                        new ScoreEntry {Name = "Player3", Frags = 33, Kills = 2, Deaths = 3},
                        new ScoreEntry {Name = "Player4", Frags = 12, Kills = 6, Deaths = 21},
                        new ScoreEntry {Name = "Player6", Frags = 12, Kills = 6, Deaths = 24},
                        new ScoreEntry {Name = "Player5", Frags = 1, Kills = 3, Deaths = 21}
                    }
                },
                new MatchInfoEntry
                {
                    Endpoint = "Server1",
                    Timestamp = DateTime.Today,
                    Map = "NewMap",
                    GameMode = "DM",
                    FragLimit = 200,
                    TimeLimit = 200,
                    TimeElapsed = 152.9,
                    Scoreboard = new List<ScoreEntry>
                    {
                        new ScoreEntry {Name = "Player1", Frags = 15, Kills = 6, Deaths = 3},
                        new ScoreEntry {Name = "Player2", Frags = 2, Kills = 2, Deaths = 21}
                    }
                },
                new MatchInfoEntry
                {
                    Endpoint = "PutServerInfo_SavesInfo",
                    Timestamp = DateTime.UtcNow.Date - TimeSpan.FromDays(3),
                    Map = "DM-HelloWorld",
                    GameMode = "TM",
                    FragLimit = 20,
                    TimeLimit = 20,
                    TimeElapsed = 12.345678,
                    Scoreboard = new List<ScoreEntry>
                    {
                        new ScoreEntry {Name = "Player1", Frags = 20, Kills = 21, Deaths = 3},
                        new ScoreEntry {Name = "Player2", Frags = 2, Kills = 2, Deaths = 21}
                    }
                }
            };
            var result = await new GameStatistics().GetRecentMatches(4);

            result.Count.ShouldBeEquivalentTo(expected.Length);
            for (var i = 0; i < result.Count; i++)
            {
                result[i].Server.ShouldBeEquivalentTo(expected[i].Endpoint);
                result[i].Timestamp.ShouldBeEquivalentTo(expected[i].Timestamp);
                result[i].Results.FragLimit.ShouldBeEquivalentTo(expected[i].FragLimit);
                result[i].Results.GameMode.ShouldBeEquivalentTo(expected[i].GameMode);
                result[i].Results.Map.ShouldBeEquivalentTo(expected[i].Map);
                result[i].Results.TimeElapsed.ShouldBeEquivalentTo(expected[i].TimeElapsed);
                result[i].Results.TimeLimit.ShouldBeEquivalentTo(expected[i].TimeLimit);
                for (var j = 0; j < result[i].Results.Scoreboard.Count; j++)
                    result[i].Results.Scoreboard[j].ShouldBeEquivalentTo(expected[i].Scoreboard[j]);
            }

        }

        [Test]
        [Order(800)]
        public async Task GetBestPlayers_DoesNotCountPlayersWithoutDeaths()
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


            await statistics.PutServerInfo(data.Endpoint, new ServerInfoEntry { Name = data.Name, GameModes = data.GameModes });
            await statistics.PutMatchInfo(data.Endpoint, time, match);


            new GameStatistics().GetBestPlayers(50).Result.Should().BeEmpty();
        }

        [Test]
        [Order(900)]
        public async Task GetBestPlayers_DoesNotCountPlayersWithLessThen10Games()
        {
            var expected = new BestPlayer[0];
            var result = await statistics.GetBestPlayers(50);
            result.ShouldBeEquivalentTo(expected);
        }

        [Test]
        [Order(1000)]
        public async Task GetBestPlayers_ReturnsCorrectStatistics()
        {
            var date = DateTime.Now.Date;
            var data = new ServerInfoEntry
            {
                Endpoint = "GetBestPlayers_ReturnsCorrectStatistics",
                Name = "Test",
                GameModes = new List<StringEntry> { new StringEntry { String = "DM" }, new StringEntry { String = "TDM" } }
            };

            var expected = new List<BestPlayerEntry>
            {
                new BestPlayerEntry {Name = "GetBestPlayers_ReturnsCorrectStatistics1", KillToDeathRatio = 20 / 4.0},
                new BestPlayerEntry {Name = "GetBestPlayers_ReturnsCorrectStatistics2", KillToDeathRatio = 2 / 20.0}
            };


            await statistics.PutServerInfo(data.Endpoint, new ServerInfoEntry { Name = data.Name, GameModes = data.GameModes });
            for (var i = 0; i < 20; i++)
            {
                var timestamp = date + TimeSpan.FromHours(i);
                await statistics.PutMatchInfo(data.Endpoint, timestamp, new MatchInfoEntry
                {
                    Map = "1",
                    GameMode = "2",
                    FragLimit = 20,
                    TimeLimit = 300,
                    TimeElapsed = 25,
                    Scoreboard = new List<ScoreEntry>
                    {
                        new ScoreEntry
                        {
                            Name = "GetBestPlayers_ReturnsCorrectStatistics1",
                            Deaths = 4,
                            Frags = 2,
                            Kills = 20
                        },
                        new ScoreEntry
                        {
                            Name = "GetBestPlayers_ReturnsCorrectStatistics2",
                            Deaths = 20,
                            Frags = 20,
                            Kills = 2
                        }
                    }
                });
            }

            var result = statistics.GetBestPlayers(2).Result.ToList();


            result.Count.ShouldBeEquivalentTo(expected.Count);
            for (var i = 0; i < expected.Count; i++)
                result[i].ShouldBeEquivalentTo(expected[i]);
        }

        [Test]
        [Order(1100)]
        public async Task GetPopularServers_ReturnsCorrectStatistics()
        {
            var date = DateTime.Now.Date;
            var server = new ServerInfoEntry
            {
                Endpoint = "GetPopularServers_ReturnsCorrectStatistics1",
                Name = "Server1",
                GameModes =
                    new List<StringEntry> {new StringEntry {String = "DM"}, new StringEntry {String = "TDM"}}
            };

            var expected = new List<PopularServerEntry>
            {
                new PopularServerEntry { Endpoint = "GetBestPlayers_ReturnsCorrectStatistics", Name = "Test", AverageMatchesPerDay = 20},
                new PopularServerEntry {Endpoint = "GetPopularServers_ReturnsCorrectStatistics1", Name = "Server1", AverageMatchesPerDay = 15}
            };


            await statistics.PutServerInfo(server.Endpoint, new ServerInfoEntry { Name = server.Name, GameModes = server.GameModes });
            for (var i = 0; i < 15; i++)
            {
                await statistics.PutMatchInfo(server.Endpoint, date + TimeSpan.FromHours(i), new MatchInfoEntry
                {
                    Map = "1",
                    GameMode = "2",
                    FragLimit = 20,
                    TimeLimit = 300,
                    TimeElapsed = 25,
                    Scoreboard = new List<ScoreEntry>
                    {
                        new ScoreEntry
                        {
                            Name = "GetPopularServers_ReturnsCorrectStatistics1",
                            Deaths = 1,
                            Frags = 2,
                            Kills = 20
                        },
                        new ScoreEntry
                        {
                            Name = "GetPopularServers_ReturnsCorrectStatistics2",
                            Deaths = 1,
                            Frags = 20,
                            Kills = 2
                        }
                    }
                });
            }
            var result = statistics.GetPopularServers(2).Result.ToList();


            result.Count.ShouldBeEquivalentTo(expected.Count);
            for (var i = 0; i < expected.Count; i++)
                result[i].ShouldBeEquivalentTo(expected[i]);
        }
    }
}
