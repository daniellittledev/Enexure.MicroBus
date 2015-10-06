using System.Threading.Tasks;

namespace Enexure.MicroBus.Sagas
{
	public class SagaRunnerEventHandler<TSaga, TEvent> : IEventHandler<TEvent>
		where TEvent : IEvent
		where TSaga : ISaga
	{
		private readonly ISagaRepository sagaRepository;
		private readonly IDependencyScope scope;

		public SagaRunnerEventHandler(ISagaRepository sagaRepository, IDependencyScope scope)
		{
			this.scope = scope;
			this.sagaRepository = sagaRepository;
		}

		public async Task Handle(TEvent @event)
		{
			var saga = await GetSagaForAsync(@event);
			var isNew = (saga == null);

			if (isNew) {
				saga = scope.GetService<TSaga>();
			} 

			// ReSharper disable once SuspiciousTypeConversion.Global
			var sagaHandle = (IEventHandler<TEvent>)saga;
			await sagaHandle.Handle(@event);

			if (isNew) {
				await sagaRepository.CreateAsync(saga);

			} else if (saga.IsCompleted) {
				await sagaRepository.CompleteAsync(saga);

			} else {
				await sagaRepository.UpdateAsync(saga);
			}
		}

		private async Task<ISaga> GetSagaForAsync(TEvent message)
		{
			var isStartable = typeof(ISagaStartedBy<TEvent>).IsAssignableFrom(typeof(TSaga));

			var finder = scope.GetService<ISagaFinder<TSaga, TEvent>>();

			if (finder == null && !isStartable)
			{
				throw new NoSagaFinderRegisteredException(typeof(TSaga), typeof(TEvent));
			}

			if (finder == null)
			{
				return null;
			}

			var saga = await finder.FindByAsync(message);

			if (saga == null && !isStartable)
			{
				throw new NoSagaFoundException(typeof(TSaga), typeof(TEvent));
			}

			return saga;
		}
	}
}