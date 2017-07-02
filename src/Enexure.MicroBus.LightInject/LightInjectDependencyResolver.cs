using System;
using LightInject;

namespace Enexure.MicroBus.LightInject
{
    internal class LightInjectDependencyResolver : IDependencyResolver
    {
        private readonly IServiceFactory serviceFactory;
        private readonly IMarker marker;

        public LightInjectDependencyResolver(IServiceFactory serviceFactory, IMarker marker)
        {
            this.serviceFactory = serviceFactory;
            this.marker = marker;
        }

        public IDependencyScope BeginScope()
        {
            if (marker.ScopeCreated)
            {
                return new LightInjectDependencyScope(serviceFactory, marker);
            }
            else
            {
                var scope = serviceFactory.BeginScope();
                var newMarker = scope.GetInstance<IMarker>();
                newMarker.ScopeCreated = true;
                return new LightInjectDependencyScope(scope, newMarker);
            }
        }
    }
}