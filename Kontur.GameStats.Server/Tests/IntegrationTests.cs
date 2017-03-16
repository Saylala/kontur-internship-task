using System;
using System.IO;
using System.Net;
using System.Text;
using Kontur.GameStats.Server.Core;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Kontur.GameStats.Server.Tests
{
    public class IntegrationTests
    {
        private const string Prefix = "http://localhost:8080/";
        private StatServer server;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", Directory.GetCurrentDirectory());
        }

        [SetUp]
        public void SetUp()
        {
            server = new StatServer();
            server.Start(Prefix);
        }

        [TearDown]
        public void TearDown()
        {
            server.Stop();
            server.Dispose();
        }

        public void Put(string path, string data)
        {
            var request = (HttpWebRequest) WebRequest.Create(Prefix + path);
            request.Method = "PUT";
            var bytes = Encoding.UTF8.GetBytes(data);
            request.GetRequestStream().Write(bytes, 0, bytes.Length);
            request.GetResponse();
        }

        public string Get(string path)
        {
            var request = (HttpWebRequest) WebRequest.Create(Prefix + path);
            request.Method = "GET";

            var response = (HttpWebResponse)request.GetResponse();
            var data = new StreamReader(response.GetResponseStream()).ReadToEnd();

            response.Close();
            return data;
        }

        [Test]
        [Order(001)]
        public void TestGetEmptyServersInfo()
        {
            var servers = Get("/servers/info");

            AssertJsonEquals("[]", servers);
        }

        [Order(002)]
        [TestCase(-10)]
        [TestCase(5)]
        [TestCase(25)]
        [TestCase(100)]
        public void TestGetEmptyRecentMatches(int count = 5)
        {
            var number = count == 5 ? "" : $"/{count}";

            var statistics = Get($"/reports/recent-matches{number}");

            AssertJsonEquals("[]", statistics);
        }

        [Order(003)]
        [TestCase(-10)]
        [TestCase(5)]
        [TestCase(25)]
        [TestCase(100)]
        public void TestGetEmptyBestPlayers(int count)
        {
            var number = count == 5 ? "" : $"/{count}";

            var statistics = Get($"/reports/best-players{number}");

            AssertJsonEquals("[]", statistics);
        }

        [Order(004)]
        [TestCase(-10)]
        [TestCase(5)]
        [TestCase(25)]
        [TestCase(100)]
        public void TestGetEmptyPopularServers(int count)
        {
            var number = count == 5 ? "" : $"/{count}";

            var statistics = Get($"/reports/popular-servers{number}");

            AssertJsonEquals("[]", statistics);
        }

        [Test]
        [Order(100)]
        public void TestGetServerInfo()
        {
            var endpoint = "1.42.23.32-1337";
            var expectedServer = @"{
                ""name"": ""] My P3rfect Server ["",
                ""gameModes"": [ ""DM"", ""TDM"" ]
            }";

            Put($"/servers/{endpoint}/info", expectedServer);

            var resultServer = Get($"/servers/{endpoint}/info");

            AssertJsonEquals(expectedServer, resultServer);
        }

        [Test]
        [Order(200)]
        public void TestGetMatchInfo()
        {
            var endpoint = "2.42.23.32-1337";
            var server1 = @"{
                ""name"": ""] My P3rfect Server ["",
                ""gameModes"": [ ""DM"", ""TDM"" ]
            }";
            var timestamp = "2017-01-22T15:17:00Z";
            var match = @"{
                ""map"": ""DM - HelloWorld"",
                ""gameMode"": ""DM"",
                ""fragLimit"": 20,
                ""timeLimit"": 20,
                ""timeElapsed"": 12.345678,
                ""scoreboard"": [
                    {
                        ""name"": ""Player1"",
                        ""frags"": 20,
                        ""kills"": 21,
                        ""deaths"": 3
                    },
                    {
                        ""name"": ""Player2"",
                        ""frags"": 2,
                        ""kills"": 2,
                        ""deaths"": 21
                    }
                ]
            }";

            Put($"/servers/{endpoint}/info", server1);
            Put($"/servers/{endpoint}/matches/{timestamp}", match);

            var resultMatch = Get($"/servers/{endpoint}/matches/{timestamp}");

            AssertJsonEquals(match, resultMatch);
        }

        [Test]
        [Order(300)]
        public void TestGetServersInfo()
        {
            var expectedServers = @"[
                {
                    ""endpoint"": ""1.42.23.32-1337"",
                    ""info"": {
                        ""name"": ""] My P3rfect Server ["",
                        ""gameModes"": [ ""DM"", ""TDM"" ]
                    }
                },
                {
                    ""endpoint"": ""2.42.23.32-1337"",
                    ""info"": {
                        ""name"": ""] My P3rfect Server ["",
                        ""gameModes"": [ ""DM"", ""TDM"" ]
                    }
                }
            ]";

            var servers = Get("/servers/info");

            AssertJsonEquals(expectedServers, servers);
        }


        [Order(301)]
        [TestCase(5)]
        [TestCase(25)]
        [TestCase(100)]
        public void TestGetRecentMatches(int count)
        {
            var number = count == 5 ? "" : $"/{count}";

            var expectedStatistics = @"[{
                ""server"": ""2.42.23.32-1337"",
                ""timestamp"": ""2017-01-22T15:17:00Z"",
                ""results"":{
                    ""map"": ""DM - HelloWorld"",
                    ""gameMode"": ""DM"",
                    ""fragLimit"": 20,
                    ""timeLimit"": 20,
                    ""timeElapsed"": 12.345678,
                    ""scoreboard"": [
                        {
                            ""name"": ""Player1"",
                            ""frags"": 20,
                            ""kills"": 21,
                            ""deaths"": 3
                        },
                        {
                            ""name"": ""Player2"",
                            ""frags"": 2,
                            ""kills"": 2,
                            ""deaths"": 21
                        }
                    ]
                }
            }]";

            var statistics = Get($"/reports/recent-matches{number}");

            AssertJsonEquals(expectedStatistics, statistics);
        }

        [Order(302)]
        [TestCase(5)]
        [TestCase(25)]
        [TestCase(100)]
        public void TestGetBestPlayers(int count)
        {
            var number = count == 5 ? "" : $"/{count}";

            var expectedStatistics = @"[]";

            var statistics = Get($"/reports/best-players{number}");

            AssertJsonEquals(expectedStatistics, statistics);
        }

        [Order(303)]
        [TestCase(5)]
        [TestCase(25)]
        [TestCase(100)]
        public void TestGetPopularsServers(int count)
        {
            var number = count == 5 ? "" : $"/{count}";

            var expectedStatistics = @"[
                {
                    ""endpoint"": ""2.42.23.32-1337"",
                    ""name"": ""] My P3rfect Server ["",
                    ""averageMatchesPerDay"": 1.0
                },
            ]";

            var statistics = Get($"/reports/popular-servers{number}");

            AssertJsonEquals(expectedStatistics, statistics);
        }

        [Test]
        [Order(400)]
        public void TestGetServerStatistics()
        {
            var endpoint = "4.42.23.32-1337";
            var server1 = @"{
                ""name"": ""] My P3rfect Server ["",
                ""gameModes"": [ ""DM"", ""TDM"" ]
            }";

            var timestamp = "2017-01-22T15:17:00Z";
            var match = @"{
                ""map"": ""DM - HelloWorld"",
                ""gameMode"": ""DM"",
                ""fragLimit"": 20,
                ""timeLimit"": 20,
                ""timeElapsed"": 12.345678,
                ""scoreboard"": [
                    {
                        ""name"": ""Player1"",
                        ""frags"": 20,
                        ""kills"": 21,
                        ""deaths"": 3
                    },
                    {
                        ""name"": ""Player2"",
                        ""frags"": 2,
                        ""kills"": 2,
                        ""deaths"": 21
                    }
                ]
            }";

            var expectedStatistics = @"{
                ""totalMatchesPlayed"": 1,
                ""maximumMatchesPerDay"": 1,
                ""averageMatchesPerDay"": 1.0,
                ""maximumPopulation"": 2,
                ""averagePopulation"": 2.0,
                ""top5GameModes"": [ ""DM"",],
                ""top5Maps"": [ ""DM - HelloWorld"",]
                }";

            Put($"/servers/{endpoint}/info", server1);
            Put($"/servers/{endpoint}/matches/{timestamp}", match);
            var statistics = Get($"/servers/{endpoint}/stats");

            AssertJsonEquals(expectedStatistics, statistics);
        }

        [Test]
        [Order(500)]
        public void TestGetPlayerStatistics()
        {
            var endpoint = "5.42.23.32-1337";
            var server1 = @"{
                ""name"": ""] My P3rfect Server ["",
                ""gameModes"": [ ""DM"", ""TDM"" ]
            }";
            var playerName = "VasyanPRO";
            var timestamp = "2017-01-22T15:17:00Z";
            var match = @"{
                ""map"": ""DM - HelloWorld"",
                ""gameMode"": ""DM"",
                ""fragLimit"": 20,
                ""timeLimit"": 20,
                ""timeElapsed"": 12.345678,
                ""scoreboard"": [
                    {
                        ""name"": ""VasyanPRO"",
                        ""frags"": 20,
                        ""kills"": 21,
                        ""deaths"": 3
                    },
                    {
                        ""name"": ""Player2"",
                        ""frags"": 2,
                        ""kills"": 2,
                        ""deaths"": 21
                    }
                ]
            }";

            var expectedStatistics = @"{
                ""totalMatchesPlayed"": 1,
                ""totalMatchesWon"": 1,
                ""favoriteServer"": ""5.42.23.32-1337"",
                ""uniqueServers"": 1,
                ""favoriteGameMode"": ""DM"",
                ""averageScoreboardPercent"": 100.0,
                ""maximumMatchesPerDay"": 1,
                ""averageMatchesPerDay"": 1.0,
                ""lastMatchPlayed"": ""2017-01-22T15:17:00Z"",
                ""killToDeathRatio"": 0.0
                }";

            Put($"/servers/{endpoint}/info", server1);
            Put($"/servers/{endpoint}/matches/{timestamp}", match);
            var statistics = Get($"/players/{playerName}/stats");

            AssertJsonEquals(expectedStatistics, statistics);
        }

        [Test]
        [Order(600)]
        public void TestUpdatedGetServerInfo()
        {
            var endpoint = "1.42.23.32-1337";
            var newServerData = @"{
                ""name"": ""new Name"",
                ""gameModes"": [ ""new Game Mode"", ""DM"" ]
            }";

            Put($"/servers/{endpoint}/info", newServerData);

            var resultServer = Get($"/servers/{endpoint}/info");

            AssertJsonEquals(newServerData, resultServer);
        }

        [Test]
        [Order(601)]
        public void TestUnadvertisedServerPutsMatch()
        {
            var endpoint = "Nonexistent-Endpoint";
            var timestamp = "2017-01-22T15:17:00Z";
            var match = @"{
                ""map"": ""DM - HelloWorld"",
                ""gameMode"": ""DM"",
                ""fragLimit"": 20,
                ""timeLimit"": 20,
                ""timeElapsed"": 12.345678,
                ""scoreboard"": [
                    {
                        ""name"": ""VasyanPRO"",
                        ""frags"": 20,
                        ""kills"": 21,
                        ""deaths"": 3
                    },
                    {
                        ""name"": ""Player2"",
                        ""frags"": 2,
                        ""kills"": 2,
                        ""deaths"": 21
                    }
                ]
            }";

            try
            {
                Put($"/servers/{endpoint}/matches/{timestamp}", match);
            }
            catch (Exception e)
            {
                Assert.AreEqual("The remote server returned an error: (400) Bad Request.", e.Message);
            }
        }


        [Test]
        [Order(602)]
        public void TestGetServerInfoNotFoundEndpointNotExists()
        {
            var endpoint = "Nonexistent-Endpoint";

            try
            {
                Get($"/servers/{endpoint}/info");
            }
            catch (Exception e)
            {
                Assert.AreEqual("The remote server returned an error: (404) Not Found.", e.Message);
            }
        }

        [Test]
        [Order(603)]
        public void TestGetMatchInfoNotFoundMatchNotExists()
        {
            var timestamp = "207-01-22T15:17:00Z";
            var endpoint = "1.42.23.32-1337";

            try
            {
                Get($"/servers/{endpoint}/matches/{timestamp}");
            }
            catch (Exception e)
            {
                Assert.AreEqual("The remote server returned an error: (404) Not Found.", e.Message);
            }
        }

        [Test]
        [Order(604)]
        public void TestInvalidRoute()
        {
            try
            {
                Get("SomeInvalidRoute");
            }
            catch (Exception e)
            {
                Assert.AreEqual("The remote server returned an error: (400) Bad Request.", e.Message);
            }
        }

        [Test]
        [Order(605)]
        public void TestPutExistingMatch()
        {
            var endpoint = "5.42.23.32-1337";
            var playerName = "VasyanPRO";
            var timestamp = "2017-01-22T15:17:00Z";
            var match = @"{
                ""map"": ""Something"",
                ""gameMode"": ""Test"",
                ""fragLimit"": 1,
                ""timeLimit"": 2,
                ""timeElapsed"": 12.345678,
                ""scoreboard"": [
                    {
                        ""name"": ""Eee666"",
                        ""frags"": 20,
                        ""kills"": 21,
                        ""deaths"": 3
                    },
                    {
                        ""name"": ""Rock666"",
                        ""frags"": 21,
                        ""kills"": 27,
                        ""deaths"": 21
                    }
                ]
            }";

            try
            {
                Put($"/servers/{endpoint}/matches/{timestamp}", match);
            }
            catch (Exception e)
            {
                Assert.AreEqual("The remote server returned an error: (400) Bad Request.", e.Message);
            }
        }



        private void AssertJsonEquals(string expected, string actual)
        {
            Assert.AreEqual(NormalizeJson(expected), NormalizeJson(actual));
        }

        private static string NormalizeJson(string json)
        {
            return JsonConvert.SerializeObject(JsonConvert.DeserializeObject(json), Formatting.Indented);
        }
    }
}
