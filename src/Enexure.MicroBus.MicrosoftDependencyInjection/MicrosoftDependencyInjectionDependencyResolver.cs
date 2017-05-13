using System;
using Microsoft.Extensions.DependencyInjection;

namespace Enexure.MicroBus.MicrosoftDependencyInjection
{
    internal class MicrosoftDependencyInjectionDependencyResolver : IDependencyResolver
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IMarker marker;

        public MicrosoftDependencyInjectionDependencyResolver(IServiceProvider serviceProvider, IMarker marker)
        {
            this.serviceProvider = serviceProvider;
            this.marker = marker;
        }

        public IDependencyScope BeginScope()
        {
            if (marker.ScopeCreated)
            {
                return new MicrosoftDependencyInjectionDependencyScope(serviceProvider, marker);
            }
            else
            {
                var scope = serviceProvider.CreateScope();
                var provider = scope.ServiceProvider;
                var newMarker = provider.GetService<IMarker>();
                newMarker.ScopeCreated = true;
                return new MicrosoftDependencyInjectionDependencyScope(provider, newMarker);
            }
        }
    }
}