using System;
using Autofac;
using Enexure.MicroBus.InfrastructureContracts;

namespace Enexure.MicroBus.Autofac
{
	public static class ContainerExtensions
	{
		public static void RegisterMicroBus(this ContainerBuilder containerBuilder, Action<AutofacBusBuilder> registerHandlers)
		{
			var builder = new AutofacBusBuilder(containerBuilder);
			registerHandlers(builder);
			var registar = builder.Build();

			containerBuilder.RegisterInstance(registar).As<IHandlerRegistar>().SingleInstance();
			containerBuilder.RegisterType<HandlerBuilder>().As<IHandlerBuilder>();
			containerBuilder.RegisterType<AutofacHandlerActivator>().As<IHandlerActivator>();
			containerBuilder.RegisterType<MicroBus>().As<IBus>();
		}
	}
}
