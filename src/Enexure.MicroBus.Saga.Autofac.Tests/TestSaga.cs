using System;
using System.Threading.Tasks;
using Enexure.MicroBus.Sagas;

namespace Enexure.MicroBus.Saga.Autofac.Tests
{
	public class TestSaga : ISaga, ISagaStartedBy<SagaStartingAEvent>, ISagaStartedBy<SagaStartingBEvent>, IEventHandler<SagaEndingEvent>
	{
		public Guid Id { get; protected set; }
		public bool IsCompleted { get; protected set; }

		public Task Handle(SagaStartingAEvent @event)
		{
			Id = @event.CorrelationId;

			return Task.FromResult(0);
		}

		public Task Handle(SagaStartingBEvent @event)
		{
			Id = @event.CorrelationId;

			return Task.FromResult(0);
		}

		public Task Handle(SagaEndingEvent @event)
		{
			IsCompleted = true; 

			return Task.FromResult(0);
		}
	}

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