using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Enexure.MicroBus.Sagas.Repositories
{
	public class InMemorySagaRepository : ISagaRepository
	{
		private readonly IDictionary<Guid, ISaga> sagas;

		public InMemorySagaRepository()
		{
			sagas = new Dictionary<Guid, ISaga>();
		}

		public Task CreateAsync(ISaga saga)
		{
			sagas.Add(saga.Id, saga);

			return Task.FromResult(0);
		}

		public Task UpdateAsync(ISaga saga)
		{
			return Task.FromResult(0);
		}

		public Task CompleteAsync(ISaga saga)
		{
			sagas.Remove(saga.Id);

			return Task.FromResult(0);
		}

		public Task<ISaga> GetAsync(Guid sagaId)
		{
			ISaga saga;
			if (sagas.TryGetValue(sagaId, out saga)) {
				return Task.FromResult(saga);
			}

			return Task.FromResult<ISaga>(null);
		}

		public Task<ISaga> FindAsync(Expression<Func<ISaga, bool>> predicate)
		{
			return Task.FromResult(sagas.Values.AsQueryable().SingleOrDefault(predicate));
		}

	}
}
