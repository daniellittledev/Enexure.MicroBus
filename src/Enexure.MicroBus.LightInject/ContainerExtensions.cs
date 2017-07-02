using System.Linq;
using System.Reflection;
using LightInject;

namespace Enexure.MicroBus.LightInject
{
    public static class ContainerExtensions
    {
        public static IServiceContainer RegisterMicroBus(this IServiceContainer containerBuilder, BusBuilder busBuilder)
        {
            return RegisterMicroBus(containerBuilder, busBuilder, new BusSettings());
        }

        public static IServiceContainer RegisterMicroBus(this IServiceContainer containerBuilder, BusBuilder busBuilder, BusSettings busSettings)
        {
            containerBuilder.RegisterInstance(busSettings); //Singleton

            var pipelineBuilder = new PipelineBuilder(busBuilder);
            pipelineBuilder.Validate();
            RegisterHandlersWithAutofac(containerBuilder, busBuilder);

            containerBuilder.RegisterInstance<IPipelineBuilder>(pipelineBuilder); //Singleton

            containerBuilder.Register<IMarker, Marker>(new PerContainerLifetime());

            containerBuilder.Register<OuterPipelineDetector, OuterPipelineDetector>(new PerContainerLifetime());
            containerBuilder.RegisterConstructorDependency<IOuterPipelineDetector>((f, p) => f.GetInstance<OuterPipelineDetector>());
            containerBuilder.RegisterConstructorDependency<IOuterPipelineDetertorUpdater>((f, p) => f.GetInstance<OuterPipelineDetector>());

            containerBuilder.Register<IPipelineRunBuilder, PipelineRunBuilder>(new PerScopeLifetime());

            containerBuilder.Register<LightInjectDependencyScope, LightInjectDependencyScope>(new PerScopeLifetime());
            containerBuilder.RegisterConstructorDependency<IDependencyResolver>((f, p) => f.GetInstance<LightInjectDependencyScope>());
            containerBuilder.RegisterConstructorDependency<IDependencyScope>((f, p) => f.GetInstance<LightInjectDependencyScope>());

            containerBuilder.Register<IMicroBus, MicroBus>();
            containerBuilder.Register<IMicroMediator, MicroMediator>();
            containerBuilder.Register<ICancelableMicroBus, MicroBus>();
            containerBuilder.Register<ICancelableMicroMediator, MicroMediator>();

            containerBuilder.RegisterInstance<IServiceFactory>(containerBuilder.BeginScope());

            return containerBuilder;
        }

        private static void RegisterHandlersWithAutofac(IServiceContainer containerBuilder, BusBuilder busBuilder)
        {
            foreach (var globalHandlerRegistration in busBuilder.GlobalHandlerRegistrations)
            {
                containerBuilder.Register(globalHandlerRegistration.HandlerType);

                foreach (var dependency in globalHandlerRegistration.Dependencies)
                {
                    containerBuilder.Register(dependency);
                }
            }

            foreach (var registration in busBuilder.MessageHandlerRegistrations)
            {
                containerBuilder.Register(registration.HandlerType);

                foreach (var dependency in registration.Dependencies)
                {
                    containerBuilder.Register(dependency);
                    var interfaces = dependency.GetTypeInfo().ImplementedInterfaces;
                    foreach (var @interface in interfaces)
                    {
                        containerBuilder.Register(@interface, dependency);
                    }
                }
            }
        }

    }
}
