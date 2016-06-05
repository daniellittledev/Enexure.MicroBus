using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Enexure.MicroBus.Sagas
{

	public class FinderList : IEnumerable<Type>
	{
		readonly List<Type> sagaFinders = new List<Type>();

		public static FinderList Empty => new FinderList();

		public FinderList AddSagaFinder<TSagaFinder>()
		{
			var isActuallyASagaFinder = typeof(TSagaFinder)
				.GetTypeInfo()
				.ImplementedInterfaces
				.Where(i => i.GetTypeInfo().IsGenericType)
				.Select(i => i.GetGenericTypeDefinition())
				.Contains(typeof(ISagaFinder<,>));

			if (!isActuallyASagaFinder)
			{
				throw new ArgumentException($"The Saga finder you passed in must implement the interface TSagaFinder", nameof(TSagaFinder));
			}

			sagaFinders.Add(typeof(TSagaFinder));

			return this;
		}

		public IEnumerator<Type> GetEnumerator()
		{
			return sagaFinders.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return sagaFinders.GetEnumerator();
		}
	}
}
