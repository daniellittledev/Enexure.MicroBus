using Autofac;
using Enexure.MicroBus.Autofac;
using Enexure.MicroBus.Sagas;
using Xunit;
using System;
using System.Threading.Tasks;
using Enexure.MicroBus.Sagas.Autofac;
using FluentAssertions;

namespace Enexure.MicroBus.Saga.Tests
{
	public class SagaTests
	{
		private readonly Guid id = Guid.NewGuid();

		[Fact]
		public async Task StartingASaga()
		{
			var builder = new ContainerBuilder();

			var container = builder
				.RegisterSagas()
				.RegisterMicroBus(busBuilder => busBuilder.RegisterSaga<TestSaga>(new FinderList()
					.AddSagaFinder<FinderA>()
					.AddSagaFinder<FinderB>()
					.AddSagaFinder<FinderC>()
				))
				.Build();

			var bus = container.Resolve<IMicroBus>();

			await bus.Publish(new SagaStartingAEvent() { Identifier = id });

			await bus.Publish(new SagaEndingEvent() { Identifier = id });

		}

		[Fact]
		public async Task SagaWithTwoStarters()
		{
			var builder = new ContainerBuilder();

			var container = builder
				.RegisterSagas()
				.RegisterMicroBus(busBuilder => busBuilder.RegisterSaga<TestSaga>())
				.RegisterSagaFinder<FinderA>()
				.RegisterSagaFinder<FinderB>()
				.RegisterSagaFinder<FinderC>()
				.Build();

			var bus = container.Resolve<IMicroBus>();

			await bus.Publish(new SagaStartingBEvent() { Identifier = id });

			await bus.Publish(new SagaStartingAEvent() { Identifier = id });

			await bus.Publish(new SagaEndingEvent() { Identifier = id });

		}

		[Fact]
		public void SagaHandlingAMessageBeforeItsBeenStarted()
		{
			var builder = new ContainerBuilder();

			var container = builder
				.RegisterSagas()
				.RegisterMicroBus(busBuilder => busBuilder.RegisterSaga<TestSaga>())
				.RegisterSagaFinder<FinderA>()
				.RegisterSagaFinder<FinderB>()
				.RegisterSagaFinder<FinderC>()
				.Build();

			var bus = container.Resolve<IMicroBus>();

			Func<Task> func = () => bus.Publish(new SagaEndingEvent() { Identifier = id });

			func.ShouldThrowExactly<NoSagaFoundException>();

		}

	}
}