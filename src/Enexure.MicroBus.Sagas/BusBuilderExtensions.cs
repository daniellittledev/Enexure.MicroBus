using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Enexure.MicroBus.Sagas
{
	public static class BusBuilderExtensions
	{
		public static IMessageRegister RegisterSaga<TSaga, TSagaData>(this IMessageRegister MessageRegister, TSaga saga)
			where TSaga : ISaga<TSagaData>
			where TSagaData : class
		{

			// Register a special event handler that will delegate to the Saga
			var sagaType = saga.GetType();
			var handleMethods = sagaType.GetMethods(BindingFlags.Instance | BindingFlags.Public).Where(x => x.Name == "Handle");

			handleMethods.Select(x => x.GetParameters()[0].ParameterType);

			return MessageRegister;
		}
	}

	// Note: if we could register to IEvent this could be different
	
	// Register <IEvent>().To<GenericHandler>

	// On lookup in HandlerRegistar step up through base types until IEvent and concat the handlers.

	// Validation for each registration do the same thing, the pipeline should be the same.

	// Also check the pipelines are the same when running.

	public class SagaManagingEventHandler<TEvent> : IEventHandler<TEvent>
		where TEvent : IEvent
	{
		private readonly ISagaRepository sagaRepository;

		public SagaManagingEventHandler(ISagaRepository sagaRepository)
		{
			this.sagaRepository = sagaRepository;
		}

		public async Task Handle(TEvent @event)
		{
			// Get the Saga Type

			// Get the Saga Instance

			var saga = await sagaRepository.GetSagaForAsync(@event);
			var sagaHandle = (IEventHandler<TEvent>)saga;

			// Call the Handle Method for the event
			await sagaHandle.Handle(@event);

			// Update or Complete
			if (saga.IsCompleted) {
			}

			throw new NotImplementedException();
		}
	}
}
