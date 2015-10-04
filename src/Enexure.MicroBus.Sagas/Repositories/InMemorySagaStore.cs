using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Enexure.MicroBus.Sagas.Repositories
{
	public class InMemorySagaStore : ISagaStore
	{
		private readonly IDictionary<Guid, ISaga> sagas;

		public InMemorySagaStore()
		{
			sagas = new Dictionary<Guid, ISaga>();
		}

		public void Add(ISaga saga)
		{
			sagas.Add(saga.Id, saga);
		}

		public void Remove(ISaga saga)
		{
			sagas.Remove(saga.Id);
		}

		public IEnumerator<ISaga> GetEnumerator()
		{
			return sagas.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return sagas.Values.GetEnumerator();
		}

		public ISaga Get(Guid sagaId)
		{
			ISaga saga;
			if (sagas.TryGetValue(sagaId, out saga)) {
				return saga;
			}

			return null;
		}

		public ISaga Find(Expression<Func<ISaga, bool>> predicate)
		{
			return sagas.Values.AsQueryable().SingleOrDefault(predicate);
		}
	}
}
