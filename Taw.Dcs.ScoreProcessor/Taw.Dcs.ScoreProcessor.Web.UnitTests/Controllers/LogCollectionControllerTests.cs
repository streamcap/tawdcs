using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;
using Taw.Dcs.ScoreProcessor.Engine;
using Taw.Dcs.ScoreProcessor.Storage;
using Taw.Dcs.ScoreProcessor.Web.Controllers;

namespace Taw.Dcs.ScoreProcessor.Web.UnitTests.Controllers
{
    [TestClass]
    public class LogCollectionControllerTests
    {
        [TestMethod]
        public void TestGet()
        {
            //Arrange
            var readRepository = A.Fake<ITableStorageReadRepository>();
            var logLines = new[] { "1", "2", "3" };
            A.CallTo(() => readRepository.GetLoglines(A<string>._)).Returns(logLines);
            var cleaner = A.Fake<ILogLinesCleaner>();
            var logger = A.Fake<ILogger>();

            //Act
            var controller = new LogCollectionController(readRepository, cleaner, logger);
            var result = controller.Get();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Length);
        }
    }
}