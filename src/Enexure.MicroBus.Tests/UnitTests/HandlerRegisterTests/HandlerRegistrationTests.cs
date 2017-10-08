using FluentAssertions;
using Xunit;

namespace Enexure.MicroBus.Tests.UnitTests.HandlerRegisterTests
{
    public class HandlerRegistrationTests
    {
        [Fact]
        public void RegisterASingleCommandHandlerShouldReturnOneRegistration()
        {
            var register = new BusBuilder()
                .RegisterCommandHandler<CommandB, CommandBHandler>();

            register.MessageHandlerRegistrations.Count.Should().Be(1);
        }

        [Fact]
        public void RegisterTwoCommandsShouldReturnTwoRegistrations()
        {
            var register = new BusBuilder()
                .RegisterCommandHandler<CommandA, CommandAHandler>()
                .RegisterCommandHandler<CommandB, CommandBHandler>();

            register.MessageHandlerRegistrations.Count.Should().Be(2);
        }

        public class CommandA : ICommand { }
        public class CommandB : ICommand { }
        class CommandAHandler : CommandHandler<CommandA> { }
        class CommandBHandler : CommandHandler<CommandB> { }
    }
}
