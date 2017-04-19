using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Taw.Dcs.ScoreProcessor.Web.Controllers;

namespace Taw.Dcs.ScoreProcessor.Web.UnitTests
{
    [TestClass]
    public class WindsorDependencyResolverTests
    {
        [TestMethod]
        public void WindsorDependencyResolverTest()
        {
            var container = new WindsorContainer().Install(new ProcessorWebInstaller());
            var res = new WindsorDependencyResolver(container);
            var controllerType = typeof(GameNamesController);
            var a = res.GetService(controllerType);
            Assert.AreEqual(controllerType, a.GetType());
        }
    }
}