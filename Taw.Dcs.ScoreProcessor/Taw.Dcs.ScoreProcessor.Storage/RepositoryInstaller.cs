using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Taw.Dcs.ScoreProcessor.Storage
{
    public class RepositoryInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<ITableStorageReadRepository>()
                .ImplementedBy<TableStorageReadRepository>()
                .LifestyleTransient());

            container.Register(Component.For<ITableStorageWriteRepository>()
                .ImplementedBy<TableStorageWriteRepository>()
                .LifestyleTransient());
        }
    }
}
