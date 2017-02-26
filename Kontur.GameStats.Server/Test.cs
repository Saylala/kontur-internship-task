using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using Kontur.GameStats.Server.Attributes;
using Kontur.GameStats.Server.Database;
using Kontur.GameStats.Server.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;

namespace Kontur.GameStats.Server
{
    public class Test
    {
        private StatServer server;

        [SetUp]
        public void SetUp()
        {
            server = new StatServer();
            server.Start("http://+:8080/");
        }

        [Test]
        public void TestServer()
        {
            var url = "http://localhost:8080/servers/<endpoint>/info";
            var request = WebRequest.Create(url);
            var line = new StreamReader(request.GetResponse().GetResponseStream()).ReadToEnd();
            Assert.AreEqual("Hello, world!", line);
        }


        [Test]
        public void PutTest()
        {

            var str = @"{""name"":""] My P3rfect Server ["",""gameModes"":[""DM"",""TDM""]}";
            var url = "http://localhost:8080/servers/test/info";
            var request = WebRequest.Create(url);
            request.Method = "PUT";
            using (var writer = new StreamWriter(request.GetRequestStream()))
                writer.WriteLine(str);

            var line = new StreamReader(request.GetResponse().GetResponseStream()).ReadToEnd();

            var get = WebRequest.Create(url);
            var response = new StreamReader(get.GetResponse().GetResponseStream()).ReadToEnd();
            Assert.False(true);
        }

        [TestCase("/servers/info", ExpectedResult = "Servers")]
        [TestCase("/servers/5/info", ExpectedResult = "5")]
        [TestCase("/servers/qwer/info", ExpectedResult = "qwer")]
        public string AtrributeTest(string route)
        {
            var dict = typeof(Controller).GetMethods().Where(x => x.GetCustomAttributes<MatchAttribute>().Any()).ToDictionary(x => x.GetCustomAttributes<MatchAttribute>().Single().Route, x => x);
            var matched = dict.Keys.FirstOrDefault(x => new Regex(x).IsMatch(route));
            var groups = new Regex(matched).Match(route).Groups;
            var groupsArr = Enumerable.Range(1, groups.Count - 1).Select(x => (object)groups[x].Value).ToArray();
            var controller = new Controller();
            return (string)dict[matched].Invoke(controller, groupsArr);
        }

        [Test]
        public void Deserialization()
        {
            var str = @"{""name"":""] My P3rfect Server ["",""gameModes"":[""DM"",""TDM""]}";
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            var a = JsonConvert.DeserializeObject<ServerInfo>(str);
            var test = JsonConvert.SerializeObject(a);
            Assert.AreEqual(str, test);
        }

        [Test]
        public void Database()
        {
        }

        [Test]
        public void PutServerTest()
        {
            var data = new ServerInfo
            {
                Endpoint = "1234",
                Name = "Test",
                GameModes = new List<StringEntry> { new StringEntry { String = "DM" }, new StringEntry { String = "TDM" } }
            };
            ServerInfo result;
            AppDomain.CurrentDomain.SetData("DataDirectory", Directory.GetCurrentDirectory());
            using (var test = new DatabaseContext())
            {
                result = test.Servers.Find(data.Endpoint);
                test.PutServerInfo(data.Endpoint, new ServerInfo { Name = data.Name, GameModes = data.GameModes });
                result = test.Servers.Find(data.Endpoint);
            }
            Assert.AreEqual(data, result);
        }

        [Test]
        public void PutMatchTest()
        {
            var time = DateTime.Now;
            var data = new MatchInfo
            {
                Endpoint = "123",
                Timestamp = time,
                Map = "DM-HelloWorld",
                GameMode = "DM",
                FragLimit = 20,
                TimeLimit = 20,
                TimeElapsed = 12.345678,
                Scoreboard = new List<Score>
                {
                    new Score
                    {
                        Name = "Player1",
                        Frags = 20,
                        Kills = 21,
                        Deaths = 3
                    },
                    new Score
                    {
                        Name = "Player2",
                        Frags = 2,
                        Kills = 2,
                        Deaths = 21
                    }
                }
            };
            MatchInfo result;
            AppDomain.CurrentDomain.SetData("DataDirectory", Directory.GetCurrentDirectory());
            using (var test = new Database.DatabaseContext())
            {
                test.Database.Delete();
                var test1 = test.Matches.FirstOrDefault();
                test.PutMatchInfo(data.Endpoint, data.Timestamp, new MatchInfo
                {
                    Map = data.Map,
                    GameMode = data.GameMode,
                    FragLimit = data.FragLimit,
                    TimeLimit = data.TimeLimit,
                    TimeElapsed = data.TimeElapsed,
                    Scoreboard = data.Scoreboard
                });
                test.SaveChanges();

            }
        }
        //private void PrepareDatabase()
        //{
        //    var rnd = new Random();
        //    const int serversCount = 10000;
        //    const int daysInHistory = 14;
        //    const int matchesInDay = 100;
        //    const int playersInMatch = 100;

        //    var players = new List<Score>();
        //    for (var l = 0; l < playersInMatch; l++)
        //    {
        //        var score = playersInMatch - l;
        //        players.Add(new Score { Deaths = score, Frags = score, Kills = score, Name = l.ToString() });
        //    }

        //    var testCount = 150 * 1000;



        //    var sw = new Stopwatch();
        //    using (var model = new Database.Database())
        //    {
        //        sw.Reset();
        //        sw.Start();

        //        //model.matches.AddRange(test);

        //        for (var i = 0; i < testCount; i++)
        //        {
        //            var day = rnd.Next(1, 15);
        //            var date = new DateTime(2017, 2, day, rnd.Next(24), rnd.Next(60), rnd.Next(60));
        //            var endpoint = rnd.Next(serversCount).ToString();
        //            var fragLimit = rnd.Next(100);
        //            var gameMode = rnd.Next(10).ToString();
        //            var map = rnd.Next(100).ToString();
        //            var timeLimit = rnd.Next(10000);
        //            var timeElapsed = rnd.NextDouble() * 1000;
        //            model.matches.Add(new MatchInfoEntry
        //            {
        //                Test = Tuple.Create(endpoint, date),
        //                MatchInfo = new MatchInfo
        //                {
        //                    Id = i,
        //                    FragLimit = fragLimit,
        //                    GameMode = gameMode,
        //                    Map = map,
        //                    TimeElapsed = timeElapsed,
        //                    TimeLimit = timeLimit
        //                }
        //            });
        //        }
        //        model.SaveChanges();

        //        sw.Stop();
        //        Console.WriteLine(sw.Elapsed.TotalSeconds);
        //    }
        //}
    }
}
