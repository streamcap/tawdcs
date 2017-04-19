using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Serilog;

namespace Taw.Dcs.ScoreProcessor.Engine
{
    public abstract class LoggerInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<ILogger>()
                .UsingFactoryMethod(CreateLogger)
                .LifestyleSingleton());

        }

        protected abstract ILogger CreateLogger();
    }

    public class EngineInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<ILogLinesCleaner>().ImplementedBy<LogLinesCleaner>().LifestyleSingleton(),
                Component.For<IScoreEventsHandler>().ImplementedBy<ScoreEventsHandler>().LifestyleTransient()
                );
        }
    }
}
