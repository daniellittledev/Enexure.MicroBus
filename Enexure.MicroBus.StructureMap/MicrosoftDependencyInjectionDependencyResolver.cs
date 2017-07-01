using System;
using StructureMap;

namespace Enexure.MicroBus.StructureMap
{
    internal class MicrosoftDependencyInjectionDependencyResolver : IDependencyResolver
    {
        private readonly IContainer container;
        private readonly IMarker marker;

        public MicrosoftDependencyInjectionDependencyResolver(IContainer container, IMarker marker)
        {
            this.container = container;
            this.marker = marker;
        }

        public IDependencyScope BeginScope()
        {
            if (marker.ScopeCreated)
            {
                return new MicrosoftDependencyInjectionDependencyScope(container, marker);
            }
            else
            {
                var scope = container.CreateChildContainer();
                var newMarker = scope.GetInstance<IMarker>();
                newMarker.ScopeCreated = true;
                return new MicrosoftDependencyInjectionDependencyScope(container, newMarker);
            }
        }
    }
}