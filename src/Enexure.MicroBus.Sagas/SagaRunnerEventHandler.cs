using System.Threading.Tasks;
using System.Reflection;
using System.Linq;

namespace Enexure.MicroBus.Sagas
{
	public class SagaRunnerEventHandler<TSaga, TEvent> : IEventHandler<TEvent>
		where TEvent : IEvent
		where TSaga : class, ISaga
	{
		private readonly ISagaRepository<TSaga> sagaRepository;
		private readonly IDependencyScope scope;

		public SagaRunnerEventHandler(ISagaRepository<TSaga> sagaRepository, IDependencyScope scope)
		{
			this.scope = scope;
			this.sagaRepository = sagaRepository;
		}

		public async Task Handle(TEvent @event)
		{
			var isNew = false;
			var isStartable = typeof(ISagaStartedBy<TEvent>).GetTypeInfo().IsAssignableFrom(typeof(TSaga).GetTypeInfo());
			var sagaFinder = scope.GetServices<ISagaFinder<TSaga, TEvent>>().FirstOrDefault();

			TSaga saga;
			if (isStartable) {
				saga = await FindSaga(@event, sagaFinder);

				if (saga == null)
				{
					isNew = true;
					saga = sagaRepository.NewSaga();
				}
			} else {
				saga = await FindSaga(@event, sagaFinder);

				if (saga == null) {
					throw new NoSagaFoundException(typeof(TSaga), typeof(TEvent));
				}
			}

			// ReSharper disable once SuspiciousTypeConversion.Global
			await ((IEventHandler<TEvent>)saga).Handle(@event);

			if (!saga.IsCompleted && isNew) {
				await sagaRepository.CreateAsync(saga);

			} else if (saga.IsCompleted) {
				await sagaRepository.CompleteAsync(saga);

			} else {
				await sagaRepository.UpdateAsync(saga);
			}
		}

		private Task<TSaga> FindSaga(TEvent @event, ISagaFinder<TSaga, TEvent> sagaFinder)
		{
			return sagaFinder != null ? sagaFinder.FindByAsync(@event) : sagaRepository.FindAsync(@event);
		}
	}
}