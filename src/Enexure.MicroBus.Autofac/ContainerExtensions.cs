using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;

namespace Enexure.MicroBus.Autofac
{
	public static class ContainerExtensions
	{
		public static ContainerBuilder RegisterMicroBus(this ContainerBuilder containerBuilder,
			Func<IMessageRegister, IMessageRegister> registerHandlers)
		{
			return RegisterMicroBus(containerBuilder, registerHandlers, new BusSettings());
		}

		public static ContainerBuilder RegisterMicroBus(this ContainerBuilder containerBuilder, Func<IMessageRegister, IMessageRegister> registerHandlers, BusSettings busSettings)
		{
			var register = registerHandlers(new HandlerRegister());
			var registrations = register.GetMessageRegistrations();

			RegisterHandlersWithAutofac(containerBuilder, registrations);
			var handlerProvider = new HandlerProvider(registrations);

			containerBuilder.RegisterInstance(handlerProvider).As<IHandlerProvider>().SingleInstance();
			containerBuilder.RegisterType<PipelineBuilder>().As<IPipelineBuilder>();
			containerBuilder.RegisterType<AutofacDependencyResolver>().As<IDependencyResolver>();
			containerBuilder.RegisterType<AutofacDependencyScope>().As<IDependencyScope>();
			containerBuilder.RegisterType<MicroBus>().As<IMicroBus>();
			containerBuilder.RegisterInstance(busSettings).AsSelf();

			return containerBuilder;
		}

		public static void RegisterHandlersWithAutofac(ContainerBuilder containerBuilder, IEnumerable<MessageRegistration> registrations)
		{
			var pipelines = new List<Pipeline>();

			foreach (var registration in registrations) {

				if (!pipelines.Contains(registration.Pipeline)) {
					pipelines.Add(registration.Pipeline);
				}

				containerBuilder.RegisterType(registration.Handler).AsSelf().InstancePerLifetimeScope();
			}

			var piplelineHandlers = pipelines.SelectMany(x => x).Distinct();
			foreach (var piplelineHandler in piplelineHandlers) {
				containerBuilder.RegisterType(piplelineHandler).AsSelf().AsImplementedInterfaces().InstancePerLifetimeScope();
			}
		}
	}
}
