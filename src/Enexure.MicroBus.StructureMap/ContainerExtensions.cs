using System.Linq;
using System.Reflection;
using StructureMap;

namespace Enexure.MicroBus.StructureMap
{
    public static class ContainerExtensions
    {
        public static ConfigurationExpression RegisterMicroBus(this ConfigurationExpression configuration, BusBuilder busBuilder)
        {
            //new Container(x => x);
            return RegisterMicroBus(configuration, busBuilder, new BusSettings());
        }

        public static ConfigurationExpression RegisterMicroBus(this ConfigurationExpression configuration, BusBuilder busBuilder, BusSettings busSettings)
        {
            configuration.For<BusSettings>().Use(x => busSettings);

            var pipelineBuilder = new PipelineBuilder(busBuilder);
            pipelineBuilder.Validate();
            RegisterHandlersWithAutofac(configuration, busBuilder);

            configuration.For<IPipelineBuilder>().Use(pipelineBuilder).Singleton();

            configuration.For<IMarker>().Use<Marker>().ContainerScoped();
            configuration.For<OuterPipelineDetector>().Use<OuterPipelineDetector>().ContainerScoped();
            configuration.Forward<OuterPipelineDetector, IOuterPipelineDetector>();
            configuration.Forward<OuterPipelineDetector, IOuterPipelineDetertorUpdater>();

            configuration.For<IPipelineRunBuilder>().Use<PipelineRunBuilder>().Transient();

            configuration.For<MicrosoftDependencyInjectionDependencyScope>().Use<MicrosoftDependencyInjectionDependencyScope>().ContainerScoped();
            configuration.Forward<MicrosoftDependencyInjectionDependencyScope, IDependencyResolver>();
            configuration.Forward<MicrosoftDependencyInjectionDependencyScope, IDependencyScope>();

            configuration.For<IMicroBus>().Use<MicroBus>().Transient();
            configuration.For<IMicroMediator>().Use<MicroMediator>().Transient();
            configuration.For<ICancelableMicroBus>().Use<MicroBus>().Transient();
            configuration.For<ICancelableMicroMediator>().Use<MicroMediator>().Transient();

            //configuration.AddTransient(x => x);

            return configuration;
        }

        private static void RegisterHandlersWithAutofac(ConfigurationExpression configuration, BusBuilder busBuilder)
        {
            foreach (var globalHandlerRegistration in busBuilder.GlobalHandlerRegistrations)
            {
                configuration.For(globalHandlerRegistration.HandlerType).Use(globalHandlerRegistration.HandlerType).Transient();

                foreach (var dependency in globalHandlerRegistration.Dependencies)
                {
                    configuration.For(dependency).Use(dependency).Transient();
                }
            }

            foreach (var registration in busBuilder.MessageHandlerRegistrations)
            {
                configuration.For(registration.HandlerType).Use(registration.HandlerType).Transient();

                foreach (var dependency in registration.Dependencies)
                {
                    configuration.For(dependency).Use(dependency).Transient();
                    var interfaces = dependency.GetTypeInfo().ImplementedInterfaces;
                    foreach (var @interface in interfaces)
                    {
                        configuration.For(@interface).Use(dependency).Transient();
                    }
                }
            }
        }

    }
}
