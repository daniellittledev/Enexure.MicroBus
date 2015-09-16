using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Enexure.MicroBus.Sagas.InMemory
{
	public class InMemorySagaStore
	{
		public InMemorySagaStore()
		{
			Sagas = new List<ISaga>();
		}

		public IList<ISaga> Sagas { get; set; }
	}

	public class InMemoryRepository : ISagaRepository
	{
		private readonly InMemorySagaStore sagaStore;
		private readonly IDependencyScope scope;
		private List<ISaga> sagas = new List<ISaga>();

		public InMemoryRepository(InMemorySagaStore sagaStore, IDependencyScope scope)
		{
			this.sagaStore = sagaStore;
			this.scope = scope;
		}

		public async Task<ISaga> GetSagaForAsync<TSaga, TEvent>(TEvent message)
			where TSaga : ISaga
			where TEvent : IEvent
		{
			var isStartable = typeof(ISagaStartedBy<TEvent>).IsAssignableFrom(typeof(TSaga));

			var finder = scope.GetService<ISagaFinder<TSaga, TEvent>>();

			var saga = finder != null ? await finder.FindByAsync(message) : default(TSaga);

			if (saga == null) {
				if (isStartable) {
					return scope.GetService<TSaga>();
				}

				if (finder == null) {
					throw new NoSagaFinderIsRegisteredForNonStartingEventException(typeof(TSaga), typeof(TEvent));
				}

				throw new NoSagaFoundForNonStartingEventException(typeof(TSaga), typeof(TEvent));
			}

			return saga;
		}

		public Task UpdateAsync(ISaga saga)
		{
			// It's already in memory
			return Task.FromResult(0);
		}

		public Task CompleteAsync(ISaga saga)
		{
			sagaStore.Sagas.Remove(saga);

			return Task.FromResult(0);
		}
	}

	public class NoSagaFoundForNonStartingEventException : Exception
	{
		public NoSagaFoundForNonStartingEventException(Type type, Type type1)
		{
			throw new NotImplementedException();
		}
	}

	public class NoSagaFinderIsRegisteredForNonStartingEventException : Exception
	{
		public NoSagaFinderIsRegisteredForNonStartingEventException(Type type, Type type1)
		{
			throw new NotImplementedException();
		}
	}
}
