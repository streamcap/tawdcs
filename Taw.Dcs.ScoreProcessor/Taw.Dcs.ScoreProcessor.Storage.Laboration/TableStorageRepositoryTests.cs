using System;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;

namespace Taw.Dcs.ScoreProcessor.Storage.Laboration
{
    [TestClass]
    public class TableStorageRepositoryTests
    {
        [TestMethod]
        public void QueryScoreEventsTest()
        {
            var logger = A.Fake<ILogger>();
            var repo = new TableStorageReadRepository(logger);
            var a = repo.QueryScoreEvents("test", DateTime.Today, "yyyy-MM-dd");
            Assert.IsNotNull(a);
        }

        [TestMethod]
        public void GetGameNames()
        {
            var logger = A.Fake<ILogger>();
            var repo = new TableStorageReadRepository(logger);
            var a = repo.QueryGameNames();
            Assert.IsNotNull(a);
            Assert.AreEqual(5, a.Count);
        }
    }
}