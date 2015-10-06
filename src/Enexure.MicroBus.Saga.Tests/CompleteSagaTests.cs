using Autofac;
using Enexure.MicroBus.Autofac;
using Enexure.MicroBus.Sagas;
using Enexure.MicroBus.Sagas.Autofac;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using FluentAssertions;

namespace Enexure.MicroBus.Saga.Tests
{
	[TestFixture]
	public class SagaTests
	{
		private readonly Guid id = Guid.NewGuid();

		[Test]
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

		[Test]
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

		[Test]
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