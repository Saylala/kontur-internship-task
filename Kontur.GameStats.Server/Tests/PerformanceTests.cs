using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Kontur.GameStats.Server.Core;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Kontur.GameStats.Server.Tests
{
    public class PerformanceTests
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
            var request = (HttpWebRequest)WebRequest.Create(Prefix + path);
            request.Method = "PUT";
            var bytes = Encoding.UTF8.GetBytes(data);
            request.GetRequestStream().Write(bytes, 0, bytes.Length);
            request.GetResponse();
        }

        public string Get(string path)
        {
            var request = (HttpWebRequest)WebRequest.Create(Prefix + path);
            request.Method = "GET";

            var response = (HttpWebResponse)request.GetResponse();
            var data = new StreamReader(response.GetResponseStream()).ReadToEnd();

            response.Close();
            return data;
        }

        [TestCase(15)]
        public void TestGetMatchInfo(int count)
        {
            var endpoint = "2.42.23.32-1337";
            var server1 = @"{
                ""name"": ""] My P3rfect Server ["",
                ""gameModes"": [ ""DM"", ""TDM"" ]
            }";
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

            var tasks = new List<Task>(count);

            var sw = Stopwatch.StartNew();
            for (var i = 0; i < count; i++)
            {
                var timestamp = JsonConvert.SerializeObject(DateTime.UtcNow + TimeSpan.FromHours(i)).Replace("\"", "");
                tasks.Add(Task.Run(() => Put($"/servers/{endpoint}/matches/{timestamp}", match)));
            }
            Console.WriteLine(sw.ElapsedMilliseconds);

            Task.WhenAll(tasks).Wait();
            Console.WriteLine(sw.ElapsedMilliseconds);
        }
    }
}
