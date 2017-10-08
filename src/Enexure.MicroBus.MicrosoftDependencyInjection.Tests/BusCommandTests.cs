using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Enexure.MicroBus.Annotations;
using FluentAssertions;
using Xunit;

namespace Enexure.MicroBus.MicrosoftDependencyInjection.Tests
{
    public class MicrosoftDependencyInjectionCommandTests
    {
        class Command : ICommand { }

        [UsedImplicitly]
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

            var container = new ServiceCollection().RegisterMicroBus(busBuilder).BuildServiceProvider();

            var bus = container.GetRequiredService<IMicroBus>();
            await bus.SendAsync(new Command());
        }

        [Fact]
        public void TestMissingCommand()
        {
            var container = new ServiceCollection().RegisterMicroBus(new BusBuilder()).BuildServiceProvider();

            var bus = container.GetRequiredService<IMicroBus>();

            new Func<Task>(() => bus.SendAsync(new Command()))
                .ShouldThrow<NoRegistrationForMessageException>();

        }
    }
}
