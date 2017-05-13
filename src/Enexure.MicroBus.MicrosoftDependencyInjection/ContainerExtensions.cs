using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Enexure.MicroBus.MicrosoftDependencyInjection
{
    public static class ContainerExtensions
    {
        public static IServiceCollection RegisterMicroBus(this IServiceCollection containerBuilder, BusBuilder busBuilder)
        {
            return RegisterMicroBus(containerBuilder, busBuilder, new BusSettings());
        }

        public static IServiceCollection RegisterMicroBus(this IServiceCollection containerBuilder, BusBuilder busBuilder, BusSettings busSettings)
        {
            containerBuilder.AddSingleton(busSettings);

            var pipelineBuilder = new PipelineBuilder(busBuilder);
            pipelineBuilder.Validate();
            RegisterHandlersWithAutofac(containerBuilder, busBuilder);

            containerBuilder.AddSingleton<IPipelineBuilder>(pipelineBuilder);

            containerBuilder.AddScoped<IMarker, Marker>();

            containerBuilder.AddScoped<OuterPipelineDetector>();
            containerBuilder.AddScoped<IOuterPipelineDetector>(x => x.GetRequiredService<OuterPipelineDetector>());
            containerBuilder.AddScoped<IOuterPipelineDetertorUpdater>(x => x.GetRequiredService<OuterPipelineDetector>());

            containerBuilder.AddTransient<IPipelineRunBuilder, PipelineRunBuilder>();

            containerBuilder.AddScoped<MicrosoftDependencyInjectionDependencyScope>();
            containerBuilder.AddScoped<IDependencyResolver>(x => x.GetRequiredService<MicrosoftDependencyInjectionDependencyScope>());
            containerBuilder.AddScoped<IDependencyScope>(x => x.GetRequiredService<MicrosoftDependencyInjectionDependencyScope>());

            containerBuilder.AddTransient<IMicroBus, MicroBus>();
            containerBuilder.AddTransient<IMicroMediator, MicroMediator>();
            containerBuilder.AddTransient<ICancelableMicroBus, MicroBus>();
            containerBuilder.AddTransient<ICancelableMicroMediator, MicroMediator>();

            containerBuilder.AddTransient(x => x);

            return containerBuilder;
        }

        private static void RegisterHandlersWithAutofac(IServiceCollection containerBuilder, BusBuilder busBuilder)
        {
            foreach (var globalHandlerRegistration in busBuilder.GlobalHandlerRegistrations)
            {
                containerBuilder.AddTransient(globalHandlerRegistration.HandlerType);

                foreach (var dependency in globalHandlerRegistration.Dependencies)
                {
                    containerBuilder.AddTransient(dependency);
                }
            }

            foreach (var registration in busBuilder.MessageHandlerRegistrations)
            {
                containerBuilder.AddTransient(registration.HandlerType);

                foreach (var dependency in registration.Dependencies)
                {
                    containerBuilder.AddTransient(dependency);
                    var interfaces = dependency.GetTypeInfo().ImplementedInterfaces;
                    foreach (var @interface in interfaces)
                    {
                        containerBuilder.AddTransient(@interface, dependency);
                    }
                }
            }
        }

    }
}
