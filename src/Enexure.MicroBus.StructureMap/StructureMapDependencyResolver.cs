using System;
using StructureMap;

namespace Enexure.MicroBus.StructureMap
{
    internal class StructureMapDependencyResolver : IDependencyResolver
    {
        private readonly IContainer container;
        private readonly IMarker marker;

        public StructureMapDependencyResolver(IContainer container, IMarker marker)
        {
            this.container = container;
            this.marker = marker;
        }

        public IDependencyScope BeginScope()
        {
            if (marker.ScopeCreated)
            {
                return new StructureMapDependencyScope(container, marker);
            }
            else
            {
                var scope = container.CreateChildContainer();
                var newMarker = scope.GetInstance<IMarker>();
                newMarker.ScopeCreated = true;
                return new StructureMapDependencyScope(container, newMarker);
            }
        }
    }
}