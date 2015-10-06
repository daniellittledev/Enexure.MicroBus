using Autofac;
using Enexure.MicroBus.Sagas.Repositories;

namespace Enexure.MicroBus.Sagas.Autofac
{
	public static class ContainerExtensions
	{
		public static ContainerBuilder RegisterSagas(this ContainerBuilder containerBuilder)
		{
			containerBuilder
				.RegisterType<InMemorySagaRepository>()
				.As<ISagaRepository>()
				.SingleInstance();

			return containerBuilder;
		}

		public static ContainerBuilder RegisterSagaFinder<TSagaFinder>(this ContainerBuilder containerBuilder)
		{
			containerBuilder
				.RegisterType<TSagaFinder>()
				.AsImplementedInterfaces()
				.InstancePerLifetimeScope();

			return containerBuilder;
		}
	}
}
