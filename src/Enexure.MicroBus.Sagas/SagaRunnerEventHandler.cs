using System.Threading.Tasks;

namespace Enexure.MicroBus.Sagas
{
	public class SagaRunnerEventHandler<TSaga, TEvent> : IEventHandler<TEvent>
		where TEvent : IEvent
		where TSaga : ISaga
	{
		private readonly ISagaRepository sagaRepository;

		public SagaRunnerEventHandler(ISagaRepository sagaRepository)
		{
			this.sagaRepository = sagaRepository;
		}

		public async Task Handle(TEvent @event)
		{
			var saga = await sagaRepository.GetSagaForAsync<TSaga>(@event);

			// ReSharper disable once SuspiciousTypeConversion.Global
			var sagaHandle = (IEventHandler<TEvent>)saga;

			await sagaHandle.Handle(@event);

			if (saga.IsCompleted) {
				await sagaRepository.CompleteAsync(saga);
			} else {
				await sagaRepository.UpdateAsync(saga);
			}
		}
	}
}