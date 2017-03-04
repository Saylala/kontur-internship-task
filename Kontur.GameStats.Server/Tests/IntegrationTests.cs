using System.IO;
using System.Net;
using System.Text;
using Kontur.GameStats.Server.Core;
using Kontur.GameStats.Server.Database;
using NUnit.Framework;

namespace Kontur.GameStats.Server.Tests
{
    public class IntegrationTests
    {
        private const string prefix = "http://localhost:8080/";
        private StatServer server;

        [SetUp]
        public void SetUp()
        {
            using (var databaseContext = new DatabaseContext())
                databaseContext.Database.Delete();
            server = new StatServer();
            server.Start(prefix);
        }

        [TearDown]
        public void TearDown()
        {
            server.Stop();
            server.Dispose();
            using (var databaseContext = new DatabaseContext())
                databaseContext.Database.Delete();
        }

        public void Put(string path, string data)
        {
            var request = (HttpWebRequest) WebRequest.Create(prefix + path);
            request.Method = "PUT";
            var bytes = Encoding.UTF8.GetBytes(data);
            request.GetRequestStream().Write(bytes, 0, bytes.Length);
        }

        public string Get(string path)
        {
            var request = (HttpWebRequest) WebRequest.Create(prefix + path);
            request.Method = "GET";

            var response = (HttpWebResponse)request.GetResponse();
            var data = new StreamReader(response.GetResponseStream()).ReadToEnd();

            response.Close();
            return data;
        }

        [Test]
        public void Test()
        {
            var endpoint1 = "167.42.23.32-1337";
            var server1 = @"{
                ""name"": ""] My P3rfect Server ["",
                ""gameModes"": [ ""DM"", ""TDM"" ]
            }";

            var endpoint2 = "62.210.26.88-1337";
            var server2 = @"{
                ""name"": "">> Sniper Heaven <<"",
                ""gameModes"": [ ""DM"" ]
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

            Put($"/servers/{endpoint1}/info", server1);
            Put($"/servers/{endpoint2}/info", server2);

            var expectedServers = @"[
                {
                    ""endpoint"": ""167.42.23.32 - 1337"",
                    ""info"": {
                        ""name"": ""] My P3rfect Server ["",
                        ""gameModes"": [ ""DM"", ""TDM"" ]
                    }
                },
                {
                    ""endpoint"": ""62.210.26.88-1337"",
                    ""info"": {
                        ""name"": "" >> Sniper Heaven <<"",
                        ""gameModes"": [ ""DM"" ]
                    }
                }
            ]";

            var servers = Get("/servers/info");

            Put($"/servers/{endpoint1}/matches/{timestamp}", match);

            var info = Get($"/servers/{endpoint1}/info");
        }
    }
}
