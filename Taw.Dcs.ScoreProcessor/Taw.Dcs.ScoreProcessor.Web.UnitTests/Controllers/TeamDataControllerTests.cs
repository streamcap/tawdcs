using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web.Http;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;
using Taw.Dcs.ScoreProcessor.Engine;
using Taw.Dcs.ScoreProcessor.Models.TableEntities;
using Taw.Dcs.ScoreProcessor.Storage;
using Taw.Dcs.ScoreProcessor.Web.Controllers;
using Taw.Dcs.ScoreProcessor.Web.Models;

namespace Taw.Dcs.ScoreProcessor.Web.UnitTests.Controllers
{
    [TestClass]
    public class TeamDataControllerTests
    {
        [TestMethod]
        public void TestPost()
        {
            //Arrange
            const string dateFormat = "yyyy-MM-dd";
            ConfigurationManager.AppSettings["DateFormat"] = dateFormat;
            var gameName = "gameName;" + DateTime.Today.ToString(dateFormat);
            var logger = A.Fake<ILogger>();
            var readRepository = A.Fake<ITableStorageReadRepository>();
            var gameNames = new[] { 1, 2, 3, 4, 5 }
                .Select(i => new ScoreEvent()).ToList();
            A.CallTo(() => readRepository.QueryScoreEvents("gameName", DateTime.Today, dateFormat)).Returns(gameNames);
            var cleaner = A.Fake<ILogLinesCleaner>();

            //Act
            var controller = new TeamDataController(readRepository, cleaner, logger);
            var result = controller.Post(gameName);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Length);
            var topList = result.FirstOrDefault(t => t.Name == "Red");
            Assert.IsNotNull(topList);
        }
        [TestMethod]
        public void TestPostWithBadParameter()
        {
            //Arrange
            var gameName = "gameName";
            var logger = A.Fake<ILogger>();
            var readRepository = A.Fake<ITableStorageReadRepository>();
            var cleaner = A.Fake<ILogLinesCleaner>();

            //Act
            HttpResponseException exception = null;
            TeamData[] result = null;
            try
            {
                var controller = new TeamDataController(readRepository, cleaner, logger);
                result = controller.Post(gameName);
            }
            catch (HttpResponseException ex)
            {
                exception = ex;
            }

            //Assert that the three summary lines are returned
            Assert.IsNull(result);
            Assert.IsNotNull(exception);
            Assert.IsInstanceOfType(exception, typeof(HttpResponseException));
            Assert.AreEqual(HttpStatusCode.BadRequest, exception.Response.StatusCode);
        }
    }
}