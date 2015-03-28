using System;
using System.Collections;
using System.Collections.Generic;
using Autofac;
using Autofac.Builder;

namespace Enexure.MicroBus.Autofac
{
	public class AutofacPipeline : IEnumerable<Type>
	{
		private readonly ContainerBuilder containerBuilder;
		readonly List<Type> types = new List<Type>();

		public AutofacPipeline(ContainerBuilder containerBuilder)
		{
			this.containerBuilder = containerBuilder;
		}

		public IEnumerator<Type> GetEnumerator()
		{
			return types.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return types.GetEnumerator();
		}

		public AutofacPipeline AddHandler<T>()
			where T : IPipelineHandler
		{
			types.Add(typeof(T));

			containerBuilder.RegisterType<T>().InstancePerLifetimeScope();

			return this;
		}
	}
}
