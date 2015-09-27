using Autofac;
using Enexure.MicroBus.Sagas.Repositories;

namespace Enexure.MicroBus.Sagas.Autofac
{
	public static class ContainerExtensions
	{
		public static ContainerBuilder RegisterSagas(this ContainerBuilder containerBuilder)
		{
			containerBuilder.RegisterType<InMemoryRepository>().As<ISagaRepository>().InstancePerLifetimeScope();
			containerBuilder.RegisterType<InMemorySagaStore>().AsSelf().SingleInstance();

			return containerBuilder;
		}

		public static ContainerBuilder RegisterSagaFinder<TSagaFinder>(this ContainerBuilder containerBuilder)
		{
			containerBuilder.RegisterType(typeof(TSagaFinder)).AsImplementedInterfaces().InstancePerLifetimeScope();
			return containerBuilder;
		}
	}
}
