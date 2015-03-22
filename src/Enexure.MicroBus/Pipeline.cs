using System;
using System.Collections;
using System.Collections.Generic;
using Enexure.MicroBus.MessageContracts;

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
	}
}
