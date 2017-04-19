using System.Web.Http;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Serilog;
using Taw.Dcs.ScoreProcessor.Engine;

namespace Taw.Dcs.ScoreProcessor.Web
{
    public class WebLoggerInstaller : LoggerInstaller
    {
        protected override ILogger CreateLogger()
        {
            var logger = new LoggerConfiguration()
                .WriteTo.RollingFile("log.log", fileSizeLimitBytes: 1024 * 1024)
                .CreateLogger();
            return logger;
        }
    }

    public class ProcessorWebInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Classes.FromThisAssembly().BasedOn<ApiController>().LifestyleTransient());
        }

    }
}