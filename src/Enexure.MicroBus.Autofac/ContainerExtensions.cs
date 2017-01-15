using System.Linq;
using Autofac;

namespace Enexure.MicroBus.Autofac
{
	public static class ContainerExtensions
	{
		public static ContainerBuilder RegisterMicroBus(this ContainerBuilder containerBuilder, BusBuilder busBuilder)
		{
			return RegisterMicroBus(containerBuilder, busBuilder, new BusSettings());
		}

		public static ContainerBuilder RegisterMicroBus(this ContainerBuilder containerBuilder, BusBuilder busBuilder, BusSettings busSettings)
		{
			containerBuilder.RegisterInstance(busSettings).AsSelf().SingleInstance();

			var pipelineBuilder = new PipelineBuilder(busBuilder);
			pipelineBuilder.Validate();
			RegisterHandlersWithAutofac(containerBuilder, busBuilder);

			containerBuilder.RegisterInstance(pipelineBuilder).AsImplementedInterfaces().SingleInstance();
			containerBuilder.RegisterType<OuterPipelineDetector>().AsImplementedInterfaces().InstancePerLifetimeScope();
			containerBuilder.RegisterType<PipelineRunBuilder>().AsImplementedInterfaces();
			containerBuilder.RegisterType<AutofacDependencyScope>().AsImplementedInterfaces();

			containerBuilder.RegisterType<MicroBus>().AsImplementedInterfaces();
			containerBuilder.RegisterType<MicroMediator>().AsImplementedInterfaces();

			return containerBuilder;
		}

		private static void RegisterHandlersWithAutofac(ContainerBuilder containerBuilder, BusBuilder busBuilder)
		{
			foreach (var globalHandlerRegistration in busBuilder.GlobalHandlerRegistrations)
			{
				containerBuilder.RegisterType(globalHandlerRegistration.HandlerType).AsSelf();

				foreach (var dependency in globalHandlerRegistration.Dependencies)
				{
					containerBuilder.RegisterType(dependency).AsSelf();
				}
			}

			foreach (var registration in busBuilder.MessageHandlerRegistrations)
			{
				containerBuilder.RegisterType(registration.HandlerType).AsSelf();

				foreach (var dependency in registration.Dependencies)
				{
					containerBuilder.RegisterType(dependency).AsSelf().AsImplementedInterfaces();
				}
			}
		}

	}
}
