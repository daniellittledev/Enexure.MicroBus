using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap;

namespace Enexure.MicroBus.StructureMap
{
    internal class MicrosoftDependencyInjectionDependencyScope : MicrosoftDependencyInjectionDependencyResolver, IDependencyScope
    {
        private readonly IContainer container;

        public MicrosoftDependencyInjectionDependencyScope(IContainer container, IMarker marker)
            : base(container, marker)
        {
            this.container = container;
        }

        public object GetService(Type serviceType)
        {
            return container.GetInstance(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return container.GetAllInstances(serviceType).Cast<object>();
        }

        public T GetService<T>()
        {
            return (T)GetService(typeof(T));
        }

        public IEnumerable<T> GetServices<T>()
        {
            return GetServices(typeof(T)).Cast<T>();
        }

        public void Dispose()
        {
        }
    }
}