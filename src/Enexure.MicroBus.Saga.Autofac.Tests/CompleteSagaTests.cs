using Autofac;
using Enexure.MicroBus.Autofac;
using Enexure.MicroBus.Sagas;
using Xunit;
using System;
using System.Threading.Tasks;
using FluentAssertions;

namespace Enexure.MicroBus.Saga.Autofac.Tests
{
    public class SagaTests
    {
        private readonly Guid id = Guid.NewGuid();

        [Fact]
        public async Task StartingASaga()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<TestSagaRepository>().AsImplementedInterfaces().SingleInstance();

            var busBuilder = new BusBuilder()
                .RegisterSaga<TestSaga>();

            var container = builder
                .RegisterMicroBus(busBuilder)
                .Build();

            var bus = container.Resolve<IMicroBus>();

            await bus.PublishAsync(new SagaStartingAEvent() { CorrelationId = id });

            await bus.PublishAsync(new SagaEndingEvent() { CorrelationId = id });

        }

        [Fact]
        public async Task SagaWithTwoStarters()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<TestSagaRepository>().AsImplementedInterfaces().SingleInstance();

            var busBuilder = new BusBuilder()
                .RegisterSaga<TestSaga>();

            var container = builder
                .RegisterMicroBus(busBuilder)
                .Build();

            var bus = container.Resolve<IMicroBus>();

            await bus.PublishAsync(new SagaStartingBEvent() { CorrelationId = id });

            await bus.PublishAsync(new SagaStartingAEvent() { CorrelationId = id });

            await bus.PublishAsync(new SagaEndingEvent() { CorrelationId = id });

        }

        [Fact]
        public void SagaHandlingAMessageBeforeItsBeenStarted()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<TestSagaRepository>().AsImplementedInterfaces().SingleInstance();

            var busBuilder = new BusBuilder()
                .RegisterSaga<TestSaga>();

            var container = builder
                .RegisterMicroBus(busBuilder)
                .Build();

            var bus = container.Resolve<IMicroBus>();

            Func<Task> func = () => bus.PublishAsync(new SagaEndingEvent() { CorrelationId = id });

            func.ShouldThrowExactly<NoSagaFoundException>();

        }

    }
}