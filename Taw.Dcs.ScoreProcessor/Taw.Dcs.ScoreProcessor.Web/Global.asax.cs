using System.Web.Http;
using System.Web.Http.Dependencies;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using Taw.Dcs.ScoreProcessor.Engine;
using Taw.Dcs.ScoreProcessor.Storage;

namespace Taw.Dcs.ScoreProcessor.Web
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        private IWindsorContainer _container;

        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            ConfigureWindsor(GlobalConfiguration.Configuration);
        }

        private void ConfigureWindsor(HttpConfiguration configuration)
        {
            _container = GetContainer();
            _container.Kernel.Resolver.AddSubResolver(new CollectionResolver(_container.Kernel, true));
            IDependencyResolver dependencyResolver = new WindsorDependencyResolver(_container);
            configuration.DependencyResolver = dependencyResolver;
        }

        private static IWindsorContainer GetContainer()
        {
            return new WindsorContainer().Install(
                new EngineInstaller(),
                new RepositoryInstaller(),
                new ProcessorWebInstaller(),
                new WebLoggerInstaller()
               );
        }

        public override void Dispose()
        {
            _container.Dispose();
            base.Dispose();
        }
    }
}
