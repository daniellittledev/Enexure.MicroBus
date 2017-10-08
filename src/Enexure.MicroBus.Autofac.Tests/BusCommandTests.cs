using System;
using System.Threading.Tasks;
using Autofac;
using Enexure.MicroBus.Annotations;
using FluentAssertions;
using Xunit;

namespace Enexure.MicroBus.Autofac.Tests
{
    public class AutofacCommandTests
    {
        class Command : ICommand { }

        [UsedImplicitly]
        class CommandHandler : ICommandHandler<Command>
        {
            public Task Handle(Command command)
            {
                return Task.CompletedTask;
            }
        }

        [Fact]
        public async Task TestCommand()
        {
            var busBuilder = new BusBuilder()
                .RegisterCommandHandler<Command, CommandHandler>();

            var container = new ContainerBuilder().RegisterMicroBus(busBuilder).Build();

            var bus = container.Resolve<IMicroBus>();
            await bus.SendAsync(new Command());
        }

        [Fact]
        public void TestMissingCommand()
        {
            var container = new ContainerBuilder().RegisterMicroBus(new BusBuilder()).Build();

            var bus = container.Resolve<IMicroBus>();

            new Func<Task>(() => bus.SendAsync(new Command()))
                .ShouldThrow<NoRegistrationForMessageException>();

        }
    }
}
