using System;
using System.Collections.Generic;
using System.Linq;
using LightInject;

namespace Enexure.MicroBus.LightInject
{
    internal class LightInjectDependencyScope : LightInjectDependencyResolver, IDependencyScope
    {
        private readonly IServiceFactory serviceFactory;

        public LightInjectDependencyScope(IServiceFactory serviceFactory, IMarker marker)
            : base(serviceFactory, marker)
        {
            this.serviceFactory = serviceFactory;
        }

        public object GetService(Type serviceType)
        {
            return serviceFactory.GetInstance(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return serviceFactory.GetAllInstances(serviceType);
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