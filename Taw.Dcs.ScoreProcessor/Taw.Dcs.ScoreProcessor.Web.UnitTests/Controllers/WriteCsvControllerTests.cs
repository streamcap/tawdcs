using System.Collections.Generic;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Taw.Dcs.ScoreProcessor.Storage;
using Taw.Dcs.ScoreProcessor.Web.Controllers;
using Taw.Dcs.ScoreProcessor.Web.Models;

namespace Taw.Dcs.ScoreProcessor.Web.UnitTests.Controllers
{
    [TestClass]
    public class WriteCsvControllerTests
    {
        [TestMethod]
        public void TestPost()
        {
            //Arrange
            var separator = ',';
            var writeRepository = A.Fake<ITableStorageWriteRepository>();
            var value = new CsvInput { Csv = "a;b;c", Separator = separator };

            //Act
            var controller = new WriteCsvController(writeRepository);
            controller.Post(value);

            //Assert
            A.CallTo(() => writeRepository.InsertScoreEvents(A<IEnumerable<string>>._, separator)).MustHaveHappened();
        }

        [TestMethod]
        public void TestPostWithDefaultSeparator()
        {
            //Arrange
            var separator = default(char);
            var writeRepository = A.Fake<ITableStorageWriteRepository>();
            var value = new CsvInput { Csv = "a;b;c", Separator = separator };

            //Act
            var controller = new WriteCsvController(writeRepository);
            controller.Post(value);

            //Assert
            A.CallTo(() => writeRepository.InsertScoreEvents(A<IEnumerable<string>>._, ';')).MustHaveHappened();            
        }
    }
}