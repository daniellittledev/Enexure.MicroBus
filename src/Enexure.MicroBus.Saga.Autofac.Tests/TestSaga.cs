using System;
using System.Threading.Tasks;
using Enexure.MicroBus.Sagas;

namespace Enexure.MicroBus.Saga.Autofac.Tests
{
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public class TestSaga : ISaga, 
		ISagaStartedBy<SagaStartingAEvent>, 
		ISagaStartedBy<SagaStartingBEvent>, 
		IEventHandler<SagaEndingEvent>
	{
		public Guid Id { get; protected set; }
		public bool IsCompleted { get; protected set; }

		public async Task Handle(SagaStartingAEvent @event)
		{
			Id = @event.CorrelationId;
		}

		public async Task Handle(SagaStartingBEvent @event)
		{
			Id = @event.CorrelationId;
		}

		public async Task Handle(SagaEndingEvent @event)
		{
			IsCompleted = true;
		}
	}
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

	public interface IHaveCorrelationId
	{
		Guid CorrelationId { get; }
	}

	public class SagaStartingAEvent : IEvent, IHaveCorrelationId
	{
		public Guid CorrelationId { get; set; }
	}

	public class SagaStartingBEvent : IEvent, IHaveCorrelationId
	{
		public Guid CorrelationId { get; set; }
	}

	public class SagaEndingEvent : IEvent, IHaveCorrelationId
	{
		public Guid CorrelationId { get; set; }
	}

}