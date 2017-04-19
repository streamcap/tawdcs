using System;
using System.Linq;
using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Taw.Dcs.ScoreProcessor.Web;
using Taw.Dcs.ScoreProcessor.Web.Controllers;

namespace Taw.Dcs.ScoreProcessor.SystemTest
{
    [TestClass]
    public class WebApiSystemTest
    {
        [TestMethod]
        public void TestGetGameNames()
        {
            var container = new WindsorContainer().Install(new ProcessorWebInstaller());
            var res = new WindsorDependencyResolver(container);
            var controllerType = typeof(GameNamesController);
            var controller = res.GetService(controllerType);
            Assert.AreEqual(controllerType, controller.GetType());
            var names = ((GameNamesController)controller).Get();            
            Assert.IsNotNull(names);
            Assert.AreEqual(5, names.Length);
        }

        [TestMethod]
        public void TestGetGameSummary()
        {
            var container = new WindsorContainer().Install(new ProcessorWebInstaller());
            var resolver = new WindsorDependencyResolver(container);
            var controllerType = typeof(GameNamesController);
            var gameNamesController = resolver.GetService(controllerType);
            Assert.AreEqual(controllerType, gameNamesController.GetType());
            var gameNames = ((GameNamesController)gameNamesController).Get();
            Assert.IsNotNull(gameNames);
            Assert.AreEqual(5, gameNames.Length);

            var gameName = gameNames.First();

            controllerType = typeof(SummaryController);
            var summaryController = resolver.GetService(controllerType);
            Assert.AreEqual(controllerType, summaryController.GetType());
            var summary = ((SummaryController)summaryController).Post(gameName);            
            Assert.IsNotNull(summary);
            Assert.AreEqual(3, summary.Length);
            foreach (var line in summary)
            {
                Console.WriteLine(line);
            }

            controllerType = typeof(TeamDataController);
            var teamDataController = resolver.GetService(controllerType);
            Assert.AreEqual(controllerType, teamDataController.GetType());
            var teamData = ((TeamDataController)teamDataController).Post(gameName);
            Assert.IsNotNull(teamData);
            Assert.IsNotNull(teamData);
            Assert.AreEqual(2, teamData.Length);
        }
    }
}
