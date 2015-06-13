using System;
using Autofac;
using Enexure.MicroBus;

namespace Enexure.MicroBus.Autofac
{
	public class AutofacHandlerActivator : IHandlerActivator
	{
		private readonly IComponentContext componentContext;

		public AutofacHandlerActivator(IComponentContext componentContext)
		{
			this.componentContext = componentContext;
		}

		public T ActivateHandler<T>(Type type)
		{
			return (T)componentContext.Resolve(type);
		}

		public T ActivateHandler<T>(Type type, IPipelineHandler innerHandler)
		{
			return (T)componentContext.Resolve(type, new TypedParameter(typeof(IPipelineHandler), innerHandler));
		}
	}
}