using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Enexure.MicroBus.Tests.UnitTests.HandlerProviderTests
{
    public class CommandHandlerProviderTests
    {
        [Fact]
        public void RegisteringTwoCommandsToTheSameMessageShouldFail()
        {
            new Action(() =>
            {
                var busBuilder = new BusBuilder()
                .RegisterCommandHandler<CommandA, CommandAHandler>()
                .RegisterCommandHandler<CommandA, CommandAHandler>();

                var piplineBuilder = new PipelineBuilder(busBuilder);
                piplineBuilder.Validate();

            }).ShouldNotThrow<InvalidDuplicateRegistrationsException>();
        }

        public class CommandA : ICommand { }
        class CommandAHandler : ICommandHandler<CommandA>
        {
            public Task Handle(CommandA Command)
            {
                throw new NotSupportedException();
            }
        }

    }
}
