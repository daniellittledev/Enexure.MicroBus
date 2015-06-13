using System;
using System.Collections;
using System.Collections.Generic;

namespace Enexure.MicroBus
{
	public class Pipeline : IEnumerable<Type>
	{
		readonly List<Type> types = new List<Type>();

		public IEnumerator<Type> GetEnumerator()
		{
			return types.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return types.GetEnumerator();
		}

		public Pipeline AddHandler<T>()
			where T : IPipelineHandler
		{
			types.Add(typeof(T));
			return this;
		}

		public Pipeline AddHandlers(IEnumerable<Type> handlers)
		{
			foreach (var handler in handlers) {

				if (typeof(IPipelineHandler).IsAssignableFrom(handler)) {

					types.Add(handler);
				} else {

					throw new InvalidOperationException("Handlers must implement the IPipelineHandler interface");
				}
			}
			return this;
		}
	}
}
