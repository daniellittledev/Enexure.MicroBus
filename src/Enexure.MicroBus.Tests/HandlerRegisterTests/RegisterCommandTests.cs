using FluentAssertions;
using Xunit;

namespace Enexure.MicroBus.Tests.HandlerRegisterTests
{
	public class RegisterCommandTests
	{
		[Fact]
		public void RegisterASingleCommandHandlerShouldReturnOneRegistration()
		{
			var register = new HandlerRegister()
				.RegisterCommand<CommandB>().To<CommandBHandler>();

			register.GetMessageRegistrations().Count.Should().Be(1);
		}


		[Fact]
		public void RegisterTwoHandlersToTheSameCommandIsNotYetInvalid()
		{
			var register = new HandlerRegister()
				.RegisterCommand<CommandA>().To<CommandAHandler>()
				.RegisterCommand<CommandA>().To<OtherCommandAHandler>()
				;

			register.GetMessageRegistrations().Count.Should().Be(2);
		}

		[Fact]
		public void RegisterPolymorphicHandlerToTheSameCommandIsNotYetInvalid()
		{
			var register = new HandlerRegister()
				.RegisterCommand<CommandB>().To<CommandAHandler>()
				.RegisterCommand<CommandB>().To<CommandBHandler>()
				;

			register.GetMessageRegistrations().Count.Should().Be(2);
		}

		[Fact]
		public void RegisterTwoCommandsShouldReturnTwoRegistrations()
		{
			var register = new HandlerRegister()
				.RegisterCommand<CommandA>().To<CommandAHandler>()
				.RegisterCommand<CommandB>().To<CommandBHandler>()
				;

			register.GetMessageRegistrations().Count.Should().Be(2);
		}
	}
}
