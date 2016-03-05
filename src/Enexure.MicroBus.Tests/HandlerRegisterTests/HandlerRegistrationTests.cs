using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Enexure.MicroBus.Tests.HandlerRegisterTests
{
	public class HandlerRegistrationTests
	{
		[Fact]
		public void RegisterASingleCommandHandlerShouldReturnOneRegistration()
		{
			var register = new BusBuilder()
				.RegisterCommandHandler<CommandB, CommandBHandler>();

			register.Registrations.Count.Should().Be(1);
		}

		[Fact]
		public void RegisterTwoCommandsShouldReturnTwoRegistrations()
		{
			var register = new BusBuilder()
				.RegisterCommandHandler<CommandA, CommandAHandler>()
				.RegisterCommandHandler<CommandB, CommandBHandler>();

			register.Registrations.Count.Should().Be(2);
		}

		public class CommandA : ICommand { }
		public class CommandB : ICommand { }
		class CommandAHandler : CommandHandler<CommandA> { }
		class CommandBHandler : CommandHandler<CommandB> { }
	}
}
