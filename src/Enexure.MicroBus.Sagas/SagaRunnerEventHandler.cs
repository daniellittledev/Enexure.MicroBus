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
			var saga = await sagaRepository.GetSagaForAsync<TSaga, TEvent>(@event);
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
	}
}