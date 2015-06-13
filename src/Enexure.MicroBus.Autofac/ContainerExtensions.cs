using System;
using Autofac;

namespace Enexure.MicroBus.Autofac
{
	public static class ContainerExtensions
	{
		public static ContainerBuilder RegisterMicroBus(this ContainerBuilder containerBuilder, Action<IBusBuilder> registerHandlers)
		{
			var builder = new AutofacBusBuilder(containerBuilder);
			registerHandlers(builder);

			containerBuilder.RegisterInstance(builder.GetHandlerRegistar()).As<IHandlerRegistar>().SingleInstance();
			containerBuilder.RegisterType<HandlerBuilder>().As<IHandlerBuilder>();
			containerBuilder.RegisterType<AutofacHandlerActivator>().As<IHandlerActivator>();
			containerBuilder.RegisterType<MicroBus>().As<IMicroBus>();

			return containerBuilder;
		}
	}
}
