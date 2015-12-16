using System;
using System.Threading.Tasks;
using Enexure.MicroBus.Sagas;

namespace Enexure.MicroBus.Saga.Tests
{
	public class TestSaga : ISaga, ISagaStartedBy<SagaStartingAEvent>, ISagaStartedBy<SagaStartingBEvent>, IEventHandler<SagaEndingEvent>
	{
		public Guid Id { get; protected set; }
		public bool IsCompleted { get; protected set; }

		public Task Handle(SagaStartingAEvent @event)
		{
			Id = @event.Identifier;

			return Task.FromResult(0);
		}

		public Task Handle(SagaStartingBEvent @event)
		{
			Id = @event.Identifier;

			return Task.FromResult(0);
		}

		public Task Handle(SagaEndingEvent @event)
		{
			IsCompleted = true; 

			return Task.FromResult(0);
		}
	}

	public class FinderA : ISagaFinder<TestSaga, SagaStartingAEvent>
	{
		private readonly ISagaRepository store;

		public FinderA(ISagaRepository store)
		{
			this.store = store;
		}

		public async Task<TestSaga> FindByAsync(SagaStartingAEvent message)
		{
			return (TestSaga) await store.GetAsync(message.Identifier);
		}
	}

	public class FinderB : ISagaFinder<TestSaga, SagaStartingBEvent>
	{
		private readonly ISagaRepository store;

		public FinderB(ISagaRepository store)
		{
			this.store = store;
		}

		public async Task<TestSaga> FindByAsync(SagaStartingBEvent message)
		{
			return (TestSaga) await store.FindAsync(x => x.Id == message.Identifier);
		}
	}
	public class FinderC : ISagaFinder<TestSaga, SagaEndingEvent>
	{
		private readonly ISagaRepository store;

		public FinderC(ISagaRepository store)
		{
			this.store = store;
		}

		public async Task<TestSaga> FindByAsync(SagaEndingEvent message)
		{
			return (TestSaga) await store.FindAsync(x => x.Id == message.Identifier);
		}
	}

	public class SagaStartingAEvent : IEvent
	{
		public Guid Identifier { get; set; }
	}

	public class SagaStartingBEvent : IEvent
	{
		public Guid Identifier { get; set; }
	}

	public class SagaEndingEvent : IEvent
	{
		public Guid Identifier { get; set; }
	}

}