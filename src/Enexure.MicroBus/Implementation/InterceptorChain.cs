using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Enexure.MicroBus
{
	internal class InterceptorChain : IInterceptorChain
	{
		private Func<object, Task<IReadOnlyCollection<object>>> handle;

		public InterceptorChain(Func<object, Task<IReadOnlyCollection<object>>> handle)
		{
			this.handle = handle;
		}

		public Task<IReadOnlyCollection<object>> Handle(object message)
		{
			return handle(message);
		}
	}
}