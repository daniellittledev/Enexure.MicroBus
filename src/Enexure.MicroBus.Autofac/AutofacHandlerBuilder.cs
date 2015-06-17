using System;
using System.Collections.Generic;
using Autofac;

namespace Enexure.MicroBus.Autofac
{
	public class AutofacHandlerActivator : IHandlerActivator
	{
		private readonly IComponentContext componentContext;

		public AutofacHandlerActivator(IComponentContext componentContext)
		{
			this.componentContext = componentContext;
		}

		public T ActivateHandler<T>(Type type, IPipelineHandler innerHandler)
		{
			return (T)componentContext.Resolve(type, new TypedParameter(typeof(IPipelineHandler), innerHandler));
		}

		public IEnumerable<T> ActivateHandlers<T>(MessageRegistration registration)
		{
			return componentContext.Resolve<IEnumerable<T>>();
		}
	}
}