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
			containerBuilder.RegisterType<PipelineRunBuilder>().As<IPipelineRunBuilder>();
			containerBuilder.RegisterType<AutofacDependencyResolver>().As<IDependencyResolver>();
			containerBuilder.RegisterType<AutofacDependencyScope>().As<IDependencyScope>();

			containerBuilder.RegisterType<MicroBus>().As<IMicroBus>();
			containerBuilder.RegisterType<MicroMediator>().As<IMicroMediator>();

			return containerBuilder;
		}

		private static void RegisterHandlersWithAutofac(ContainerBuilder containerBuilder, BusBuilder busBuilder)
		{
			foreach (var interceptorRegistration in busBuilder.Interceptors)
			{
				containerBuilder.RegisterType(interceptorRegistration.InterceptorType).AsSelf();

				foreach (var dependency in interceptorRegistration.Dependencies)
				{
					containerBuilder.RegisterType(dependency).AsSelf();
				}
			}

			foreach (var registration in busBuilder.Registrations)
			{
				containerBuilder.RegisterType(registration.HandlerType).AsSelf();

				foreach (var dependency in registration.Dependencies)
				{
					containerBuilder.RegisterType(dependency).AsSelf();
				}
			}
		}

	}
}
