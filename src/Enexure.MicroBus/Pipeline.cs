using System;
using System.Collections.Generic;

namespace Enexure.MicroBus
{
	public class Pipeline
	{
		public IReadOnlyCollection<Type> InterceptorTypes { get; }
		public IReadOnlyCollection<Type> HandlerTypes { get; }

		public Pipeline(IReadOnlyCollection<Type> interceptorTypes, IReadOnlyCollection<Type> handlerTypes)
		{
			InterceptorTypes = interceptorTypes;
			HandlerTypes = handlerTypes;
		}
	}
}