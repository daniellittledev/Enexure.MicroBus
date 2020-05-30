using System;
using System.Threading.Tasks;
using LightInject;
using FluentAssertions;
using Xunit;

namespace Enexure.MicroBus.LightInject.Tests
{
    public class LightInjectCommandTests
    {
        class Command : ICommand { }

        class CommandHandler : ICommandHandler<Command>
        {
            public Task Handle(Command command)
            {
                return Task.FromResult(0);
            }
        }

        [Fact]
        public async Task TestCommand()
        {
            var busBuilder = new BusBuilder()
                .RegisterCommandHandler<Command, CommandHandler>();

            var container = new ServiceContainer();
            container.RegisterMicroBus(busBuilder);

            var bus = container.GetInstance<IMicroBus>();
            await bus.SendAsync(new Command());
        }

        [Fact]
        public void TestMissingCommand()
        {
            var container = new ServiceContainer();
            container.RegisterMicroBus(new BusBuilder());

            var bus = container.GetInstance<IMicroBus>();

            new Func<Task>(() => bus.SendAsync(new Command()))
                .ShouldThrow<NoRegistrationForMessageException>();

        }
    }
}
