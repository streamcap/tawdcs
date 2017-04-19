using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;
using Taw.Dcs.ScoreProcessor.Engine;
using Taw.Dcs.ScoreProcessor.Models.TableEntities;
using Taw.Dcs.ScoreProcessor.Storage;
using Taw.Dcs.ScoreProcessor.Web.Controllers;

namespace Taw.Dcs.ScoreProcessor.Web.UnitTests.Controllers
{
    [TestClass]
    public class GameNamesControllerTests
    {
        [TestMethod]
        public void TestGet()
        {
            //Arrange
            var logger = A.Fake<ILogger>();
            var readRepository = A.Fake<ITableStorageReadRepository>();
            IList<GameNameEntity> gameNames = (new[] { 1, 2, 3, 4, 5 })
                .Select(i => new GameNameEntity(i.ToString(), DateTime.Today.AddDays(i))).ToList();
            A.CallTo(() => readRepository.QueryGameNames()).Returns(gameNames);
            var cleaner = A.Fake<ILogLinesCleaner>();

            //Act
            var controller = new GameNamesController(logger, readRepository, cleaner);
            var result = controller.Get();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Length);
        }
    }
}