using System.Collections.Generic;
using FluentAssertions;
using Kontur.GameStats.Server.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using ServerInfo = Kontur.GameStats.Server.Models.Serialization.ServerInfo;
using ServerStatistics = Kontur.GameStats.Server.Models.Serialization.ServerStatistics;

namespace Kontur.GameStats.Server.Tests
{
    public class SerializationTests
    {
        [SetUp]
        public void SetUp()
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
        }

        [Test]
        public void CorrectServerInfoSerialization()
        {
            var input = @"{""name"":""] My P3rfect Server ["",""gameModes"":[""DM"",""TDM""]}";

            var temp = JsonConvert.DeserializeObject<ServerInfo>(input);
            var result = JsonConvert.SerializeObject(temp);

            result.ShouldBeEquivalentTo(input);
        }

        [Test]
        public void CorrectSMatchInfoSerialization()
        {
            var input = @"{""map"":""DM-HelloWorld"",""gameMode"":""DM"",""fragLimit"":20,""timeLimit"":20,""timeElapsed"":12.345678,""scoreboard"":[{""name"":""Player1"",""frags"":20,""kills"":21,""deaths"":3},{""name"":""Player2"",""frags"":2,""kills"":2,""deaths"":21}]}";

            var temp = JsonConvert.DeserializeObject<MatchInfo>(input);
            var result = JsonConvert.SerializeObject(temp);

            result.ShouldBeEquivalentTo(input);
        }

        [Test]
        public void CorrectServersInfoSerialization()
        {
            var input = @"[{""endpoint"":""167.42.23.32-1337"",""info"":{""name"":""] My P3rfect Server ["",""gameModes"":[""DM"",""TDM""]}},{""endpoint"":""62.210.26.88-1337"",""info"":{""name"":"">> Sniper Heaven <<"",""gameModes"":[""DM""]}}]";

            var temp = JsonConvert.DeserializeObject<List<ServersInfo>>(input);
            var result = JsonConvert.SerializeObject(temp);

            result.ShouldBeEquivalentTo(input);
        }

        [Test]
        public void CorrectScoreSerialization()
        {
            var input = @"{""name"":""Player2"",""frags"":2,""kills"":2,""deaths"":21}";

            var temp = JsonConvert.DeserializeObject<Score>(input);
            var result = JsonConvert.SerializeObject(temp);

            result.ShouldBeEquivalentTo(input);
        }

        [Test]
        public void CorrectServerStatisticsSerialization()
        {
            var input = @"{""totalMatchesPlayed"":100500,""maximumMatchesPerDay"":33,""averageMatchesPerDay"":24.45624,""maximumPopulation"":32,""averagePopulation"":20.45,""top5GameModes"":[""DM"",""TDM""],""top5Maps"":[""DM-HelloWorld"",""DM-1on1-Rose"",""DM-Kitchen"",""DM-Camper Paradise"",""DM-Appalachian Wonderland""]}";

            var temp = JsonConvert.DeserializeObject<ServerStatistics>(input);
            var result = JsonConvert.SerializeObject(temp);

            result.ShouldBeEquivalentTo(input);
        }

        [Test]
        public void CorrectPlayerStatisticsSerialization()
        {
            var input = @"{""totalMatchesPlayed"":100500,""totalMatchesWon"":1000,""favoriteServer"":""62.210.26.88-1337"",""uniqueServers"":2,""favoriteGameMode"":""DM"",""averageScoreboardPercent"":76.145693,""maximumMatchesPerDay"":33,""averageMatchesPerDay"":24.45624,""lastMatchPlayed"":""2017-01-22T15:11:12Z"",""killToDeathRatio"":3.124333}";

            var temp = JsonConvert.DeserializeObject<PlayerStatistics>(input);
            var result = JsonConvert.SerializeObject(temp);

            result.ShouldBeEquivalentTo(input);
        }

        [Test]
        public void CorrectRecentMatchesSerialization()
        {
            var input = @"[{""server"":""62.210.26.88-1337"",""timestamp"":""2017-01-22T15:11:12Z"",""results"":{""map"":""DM-HelloWorld"",""gameMode"":""DM"",""fragLimit"":20,""timeLimit"":20,""timeElapsed"":12.345678,""scoreboard"":[{""name"":""Player1"",""frags"":20,""kills"":21,""deaths"":3},{""name"":""Player2"",""frags"":2,""kills"":2,""deaths"":21}]}}]";

            var temp = JsonConvert.DeserializeObject<List<RecentMatch>>(input);
            var result = JsonConvert.SerializeObject(temp);

            result.ShouldBeEquivalentTo(input);
        }

        [Test]
        public void CorrectBestPlayersSerialization()
        {
            var input = @"[{""name"":""Player1"",""killToDeathRatio"":3.124333}]";

            var temp = JsonConvert.DeserializeObject<List<BestPlayer>>(input);
            var result = JsonConvert.SerializeObject(temp);

            result.ShouldBeEquivalentTo(input);
        }

        [Test]
        public void CorrectPopularServersSerialization()
        {
            var input = @"[{""endpoint"":""62.210.26.88-1337"",""name"":"">> Sniper Heaven <<"",""averageMatchesPerDay"":24.45624}]";

            var temp = JsonConvert.DeserializeObject<List<PopularServer>>(input);
            var result = JsonConvert.SerializeObject(temp);

            result.ShouldBeEquivalentTo(input);
        }
    }
}
