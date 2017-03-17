using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Kontur.GameStats.Server.Exceptions;
using Kontur.GameStats.Server.Routing;
using Kontur.GameStats.Server.Routing.Attributes;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Kontur.GameStats.Server.Tests
{
    public class RouteHandlingTests
    {
        private RouteHandler<TestController> routeHandler;

        [SetUp]
        public void SetUp()
        {
            routeHandler = RouteHandler.Create(new TestController());
        }

        [Test]
        public void TestRouteHandling()
        {
            Assert.AreEqual("\"kekb\"", routeHandler.Get("/t1/route/kek"));
        }

        [Test]
        public void TestDefaultArguments()
        {
            Assert.AreEqual("\"kekkek1keke\"", routeHandler.Get("/t1/other/kek"));
            Assert.AreEqual("\"kekkek2keke\"", routeHandler.Get("/t1/other/kek/kek2"));
        }

        [Test]
        public void TestIntArgument()
        {
            Assert.AreEqual("\"kek5keke\"", routeHandler.Get("/t1/int/kek"));
            Assert.AreEqual("\"kek6keke\"", routeHandler.Get("/t1/int/kek/6"));

            try
            {
                routeHandler.Get("/t1/int/kek/kek2");
            }
            catch (Exception e)
            {
                Assert.True(e.InnerException is InvalidRequestException);
            }
        }

        [Test]
        public void TestDateArgument()
        {
            Assert.AreEqual("\"kek22-Jan-14 15:17:00keke\"", routeHandler.Get("/t/date/kek/2014-01-22T15:17:00Z"));
            Assert.AreEqual("\"kek22-Jan-15 15:17:00keke\"", routeHandler.Get("/t/nullabledate/kek/2015-01-22T15:17:00Z"));
            Assert.AreEqual("\"kek01-Jan-01 00:00:00keke\"", routeHandler.Get("/t/nullabledate/kek"));

            try
            {
                routeHandler.Get("/t/date/kek/kek2");
            }
            catch (Exception e)
            {
                Assert.True(e.InnerException is InvalidRequestException);
            }

            try
            {
                routeHandler.Get("/t/nullabledate/kek/kek2");
            }
            catch (Exception e)
            {
                Assert.True(e.InnerException is InvalidRequestException);
            }
        }

        [Test]
        public void TestCustomEntity()
        {
            Assert.AreEqual("{\"A\":\"k1\",\"B\":2}", routeHandler.Get("/t/entity/k/1"));
        }

        [Test]
        public void TestPutCustomEntry()
        {
            var entry = new TestEntity
            {
                A = "abc",
                B = 3
            };
            var route = "/test/put_test/abc";

            routeHandler.Put(route, JsonConvert.SerializeObject(entry));
            JsonConvert.DeserializeObject<TestEntity>(routeHandler.Get(route)).ShouldBeEquivalentTo(entry);
        }

        [TestCase("aa", "bb")]
        [TestCase("aa", null)]
        public void TestPutDefaultArgs(string a, string b)
        {
            var entry = new TestEntity
            {
                A = "abc",
                B = 3
            };

            var put = $"/test/put_test_default/{a}" + (string.IsNullOrEmpty(b) ? "" : "/" + b);
            var get = $"/test/put_test/{a}{(string.IsNullOrEmpty(b) ? "kek" : b)}";

            routeHandler.Put(put, JsonConvert.SerializeObject(entry));
            JsonConvert.DeserializeObject<TestEntity>(routeHandler.Get(get)).ShouldBeEquivalentTo(entry);
        }

        [Test]
        public void TestMethodThrows()
        {
            try
            {
                routeHandler.Get("/t/throws/a/b");
            }
            catch (Exception e)
            {
                Assert.True(e.InnerException is NotFoundException);
            }
        }

        [Test]
        public async Task TestAsync()
        {
            var entry = new TestEntity
            {
                A = "abc",
                B = 3
            };

            var route = "/test/put_test/abc";

            await routeHandler.PutAsync(route, JsonConvert.SerializeObject(entry));
            var a = await routeHandler.GetAsync(route);

            JsonConvert.DeserializeObject<TestEntity>(a).ShouldBeEquivalentTo(entry);
        }
    }

    internal class TestEntity
    {
        public string A { get; set; }
        public int B { get; set; }
    }

    internal class TestController
    {
        private readonly Dictionary<string, TestEntity> testEntries = new Dictionary<string, TestEntity>();

        [Put]
        [Route("/test/async/<a>")]
        public async Task PutAsync(TestEntity entry, string a)
        {
            await Task.Delay(1000);
            testEntries[a] = entry;
        }

        [Route("/test/async/<a>")]
        public async Task<TestEntity> GetAsync(string a)
        {
            await Task.Delay(1000);
            return testEntries[a];
        }

        [Put]
        [Route("/test/put_test/<a>")]
        public void Put(TestEntity entry, string a)
        {
            testEntries[a] = entry;
        }

        [Route("/test/put_test/<a>")]
        public TestEntity Get(string a)
        {
            return testEntries[a];
        }

        [Put]
        [Route("/test/put_test_default/<a>[/<b>]")]
        public void PutDefaultArgs(TestEntity entry, string a, string b = "kek")
        {
            testEntries[a + b] = entry;
        }

        [Route("/t1/route/<a>")]
        public string GetMethod(string a)
        {
            return a + "b";
        }

        [Route("/t1/other/<a>[/<b>]")]
        public string GetDefaultArgs(string a, string b = "kek1")
        {
            return a + b + "keke";
        }

        [Route("/t1/int/<a>[/<b>]")]
        public string MethodWithInt(string a, int b = 5)
        {
            return a + b + "keke";
        }

        [Route("/t/date/<a>/<b>")]
        public string MethodWithDate(string a, DateTime b)
        {
            return a + b + "keke";
        }

        [Route("/t/nullabledate/<a>[/<b>]")]
        public string MethodWithNullableDate(string a, DateTime? b = null)
        {
            return a + (b ?? DateTime.MinValue) + "keke";
        }

        [Route("/t/entity/<a>/<b>")]
        public TestEntity MetodWithReturningEntity(string a, int b)
        {
            return new TestEntity
            {
                A = a + '1',
                B = b + 1
            };
        }

        [Route("/t/throws/<a>/<b>")]
        public TestEntity MetodThrows(string a, string b)
        {
            throw new NotFoundException($"{a} {b}");
        }
    }
}
