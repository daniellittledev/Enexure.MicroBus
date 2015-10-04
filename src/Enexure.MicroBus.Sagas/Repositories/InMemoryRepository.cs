using System.Threading.Tasks;

namespace Enexure.MicroBus.Sagas.Repositories
{
	public class InMemoryRepository : ISagaRepository
	{
		private readonly ISagaStore sagaStore;
		private readonly IDependencyScope scope;

		public InMemoryRepository(ISagaStore sagaStore, IDependencyScope scope)
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

			if (finder == null && !isStartable) {
				throw new NoSagaFinderIsRegisteredForNonStartingEventException(typeof(TSaga), typeof(TEvent));
			}

			if (finder == null) {
				return null;
			}

			var saga = await finder.FindByAsync(message);

			if (saga == null && !isStartable) {
				throw new NoSagaFoundForNonStartingEventException(typeof(TSaga), typeof(TEvent));
			}

			return saga;
		}

		public Task CreateAsync(ISaga saga)
		{
			sagaStore.Add(saga);
			return Task.FromResult(0);
		}

		public Task UpdateAsync(ISaga saga)
		{
			// It's already in memory
			return Task.FromResult(0);
		}

		public Task CompleteAsync(ISaga saga)
		{
			sagaStore.Remove(saga);

			return Task.FromResult(0);
		}
	}

}
