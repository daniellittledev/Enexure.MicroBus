using System;
using System.Collections.Generic;
using System.Linq;

namespace Enexure.MicroBus.Tests.UnitTests.PipelineBuilderTests
{
    internal class TestDependencyScope : IDependencyScope
    {
        private readonly List<object> objects = new List<object>();

        public void AddObject(object obj)
        {
            objects.Add(obj);
        }

        public IDependencyScope BeginScope()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }

        public object GetService(Type serviceType)
        {
            return objects.Single(x => x.GetType() == serviceType);
        }

        public T GetService<T>()
        {
            return (T)GetService(typeof(T));
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return objects.Where(x => x.GetType() == serviceType);
        }

        public IEnumerable<T> GetServices<T>()
        {
            return GetServices(typeof(T)).Cast<T>();
        }
    }
}
