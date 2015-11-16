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
			return RegisterMicroBus(containerBuilder, registerHandlers, Pipeline.EmptyPipeline, new BusSettings());
		}

		public static ContainerBuilder RegisterMicroBus(this ContainerBuilder containerBuilder, Func<IHandlerRegister, IHandlerRegister> registerHandler, Pipeline globalPipeline)
		{
			return RegisterMicroBus(containerBuilder, registerHandler, globalPipeline, new BusSettings());
		}

		public static ContainerBuilder RegisterMicroBus(this ContainerBuilder containerBuilder, Func<IHandlerRegister, IHandlerRegister> registerHandlers, BusSettings busSettings)
		{
			return RegisterMicroBus(containerBuilder, registerHandlers, Pipeline.EmptyPipeline, busSettings);
		}

		public static ContainerBuilder RegisterMicroBus(this ContainerBuilder containerBuilder, Func<IHandlerRegister, IHandlerRegister> registerHandlers, Pipeline globalPipeline, BusSettings busSettings)
		{
			containerBuilder.RegisterInstance(busSettings).AsSelf();

			var register = registerHandlers(new HandlerRegister());
			var registrations = register.GetMessageRegistrations();
			RegisterHandlersWithAutofac(containerBuilder, registrations);
			var handlerProvider = HandlerProvider.Create(registrations);
			containerBuilder.RegisterInstance(handlerProvider).As<IHandlerProvider>().SingleInstance();

			RegisterPipelineHandlers(containerBuilder, globalPipeline);
			containerBuilder.RegisterInstance(new GlobalPipelineProvider(globalPipeline)).As<IGlobalPipelineProvider>().SingleInstance();

			containerBuilder.RegisterType<PipelineBuilder>().As<IPipelineBuilder>();
			containerBuilder.RegisterType<AutofacDependencyResolver>().As<IDependencyResolver>();
			containerBuilder.RegisterType<AutofacDependencyScope>().As<IDependencyScope>();
			containerBuilder.RegisterType<GlobalPipelineTracker>().As<IGlobalPipelineTracker>().InstancePerLifetimeScope();
			containerBuilder.RegisterType<MicroBus>().As<IMicroBus>();

			return containerBuilder;
		}

		public static void RegisterHandlersWithAutofac(ContainerBuilder containerBuilder, IReadOnlyCollection<MessageRegistration> registrations)
		{
			var handlers = registrations.Select(x => x.Handler).Distinct();
			var piplelineHandlers = registrations.Select(x => x.Pipeline).Distinct().SelectMany(x => x).Distinct();

			RegisterLeafHandlers(containerBuilder, handlers);
			RegisterPipelineHandlers(containerBuilder, piplelineHandlers);

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

		private static void RegisterLeafHandlers(ContainerBuilder containerBuilder, IEnumerable<Type> handlers)
		{
			foreach (var handlerType in handlers)
			{
				containerBuilder.RegisterType(handlerType).AsSelf().InstancePerLifetimeScope();
			}
		}

		private static void RegisterPipelineHandlers(ContainerBuilder containerBuilder, IEnumerable<Type> piplelineHandlers)
		{
			foreach (var piplelineHandler in piplelineHandlers)
			{
				containerBuilder.RegisterType(piplelineHandler).AsSelf().AsImplementedInterfaces().InstancePerLifetimeScope();
			}
		}
	}
}
