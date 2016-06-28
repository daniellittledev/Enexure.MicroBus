using Autofac;
using Enexure.MicroBus.Autofac;
using Enexure.MicroBus.Sagas;
using Xunit;
using System;
using System.Threading.Tasks;
using FluentAssertions;

namespace Enexure.MicroBus.Saga.Autofac.Tests
{
	public class SagaFinderTests
	{
		private readonly Guid id = Guid.NewGuid();

		[Fact]
		public async Task StartingASaga()
		{
			var builder = new ContainerBuilder();

			builder.RegisterType<CallTracker>().AsSelf().SingleInstance();

			builder.RegisterType<TestSagaRepository>().AsSelf().AsImplementedInterfaces().SingleInstance();

			var busBuilder = new BusBuilder()
				.RegisterSaga<TestSaga>(FinderList.Empty.AddSagaFinder<CorrelationIdSagaFinder>());

			var container = builder
				.RegisterMicroBus(busBuilder)
				.Build();

			var bus = container.Resolve<IMicroBus>();

			await bus.PublishAsync(new SagaStartingAEvent() { CorrelationId = id });

			await bus.PublishAsync(new SagaEndingEvent() { CorrelationId = id });

			var tracker = container.Resolve<CallTracker>();

			tracker.Count.Should().Be(2);
		}

		class CallTracker
		{
			public int Count { get; private set; }

			public void RecordCall()
			{
				Count += 1;
			}
		}

		class CorrelationIdSagaFinder : ISagaFinder<TestSaga, IHaveCorrelationId>
		{
			private readonly TestSagaRepository repository;
			private readonly CallTracker tracker;

			public CorrelationIdSagaFinder(TestSagaRepository repository, CallTracker tracker)
			{
				this.repository = repository;
				this.tracker = tracker;
			}

			public Task<TestSaga> FindByAsync(int message)
			{
				return null;
			}

			public Task<TestSaga> FindByAsync(IHaveCorrelationId message)
			{
				tracker.RecordCall();
				return repository.FindById(message.CorrelationId);
			}
		}
	}
}