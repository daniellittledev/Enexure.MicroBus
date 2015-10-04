using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;

namespace Enexure.MicroBus.Autofac
{
	public static class ContainerExtensions
	{
		public static ContainerBuilder RegisterMicroBus(this ContainerBuilder containerBuilder, Func<IHandlerRegister, IHandlerRegister> registerHandlers)
		{
			return RegisterMicroBus(containerBuilder, registerHandlers, new BusSettings());
		}

		public static ContainerBuilder RegisterMicroBus(this ContainerBuilder containerBuilder, Func<IHandlerRegister, IHandlerRegister> registerHandlers, BusSettings busSettings)
		{
			var register = registerHandlers(new HandlerRegister());
			var registrations = register.GetMessageRegistrations();

			RegisterHandlersWithAutofac(containerBuilder, registrations);
			var handlerProvider = HandlerProvider.Create(registrations);

			containerBuilder.RegisterInstance(handlerProvider).As<IHandlerProvider>().SingleInstance();
			containerBuilder.RegisterType<PipelineBuilder>().As<IPipelineBuilder>();
			containerBuilder.RegisterType<AutofacDependencyResolver>().As<IDependencyResolver>();
			containerBuilder.RegisterType<AutofacDependencyScope>().As<IDependencyScope>();
			containerBuilder.RegisterType<MicroBus>().As<IMicroBus>();
			containerBuilder.RegisterInstance(busSettings).AsSelf();

			return containerBuilder;
		}

		public static void RegisterHandlersWithAutofac(ContainerBuilder containerBuilder, IReadOnlyCollection<MessageRegistration> registrations)
		{
			var handlers = registrations.Select(x => x.Handler).Distinct();
			var piplelineHandlers = registrations.Select(x => x.Pipeline).Distinct().SelectMany(x => x).Distinct();

			foreach (var handlerType in handlers)
			{
				containerBuilder.RegisterType(handlerType).AsSelf().InstancePerLifetimeScope();
			}

			foreach (var piplelineHandler in piplelineHandlers)
			{
				containerBuilder.RegisterType(piplelineHandler).AsSelf().AsImplementedInterfaces().InstancePerLifetimeScope();
			}

			foreach (var registration in registrations)
			{
				if (registration.Dependancies.Any())
				{
					foreach (var dependancy in registration.Dependancies)
					{
						containerBuilder.RegisterType(dependancy).AsSelf();
					}
				}

				if (registration.ScopedDependancies.Any())
				{
					foreach (var dependancy in registration.ScopedDependancies)
					{
						containerBuilder.RegisterType(dependancy).AsSelf().AsImplementedInterfaces().InstancePerLifetimeScope();
					}
				}
			}
		}
	}
}
