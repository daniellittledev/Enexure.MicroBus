using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;

namespace Enexure.MicroBus.Autofac
{
	public static class ContainerExtensions
	{
		public static ContainerBuilder RegisterMicroBus(this ContainerBuilder containerBuilder, Func<IBusBuilder, IBusBuilder> registerHandlers)
		{
			var builder = registerHandlers(new BusBuilder());

			RegisterHandlersWithAutofac(containerBuilder, builder);

			containerBuilder.RegisterInstance(builder.BuildHandlerRegistar()).As<IHandlerRegistar>().SingleInstance();
			containerBuilder.RegisterType<HandlerBuilder>().As<IHandlerBuilder>();
			containerBuilder.RegisterType<AutofacDependencyResolver>().As<IDependencyResolver>();
			containerBuilder.RegisterType<AutofacDependencyScope>().As<IDependencyScope>();
			containerBuilder.RegisterType<MicroBus>().As<IMicroBus>();

			return containerBuilder;
		}

		public static void RegisterHandlersWithAutofac(ContainerBuilder containerBuilder, IBusBuilder builder)
		{
			var pipelines = new List<Pipeline>();

			foreach (var registration in builder.GetMessageRegistrations()) {

				if (!pipelines.Contains(registration.Pipeline)) {
					pipelines.Add(registration.Pipeline);
				}

				foreach (var handlerType in registration.Handlers) {
					containerBuilder.RegisterType(handlerType).As(handlerType).InstancePerLifetimeScope();
				}
			}

			var piplelineHandlers = pipelines.SelectMany(x => x).Distinct();
			foreach (var piplelineHandler in piplelineHandlers) {
				containerBuilder.RegisterType(piplelineHandler).As(piplelineHandler).InstancePerLifetimeScope();
			}
		}
	}
}
