using FluentAssertions;
using NUnit.Framework;

namespace Enexure.MicroBus.Tests.HandlerRegisterTests
{
	[TestFixture]
	public class RegisterCommandTests
	{
		[Test]
		public void RegisterASingleCommandHandlerShouldReturnOneRegistration()
		{
			var register = new HandlerRegister()
				.RegisterCommand<CommandB>().To<CommandBHandler>();

			register.GetMessageRegistrations().Count.Should().Be(1);
		}


		[Test]
		public void RegisterTwoHandlersToTheSameCommandIsNotYetInvalid()
		{
			var register = new HandlerRegister()
				.RegisterCommand<CommandA>().To<CommandAHandler>()
				.RegisterCommand<CommandA>().To<OtherCommandAHandler>()
				;

			register.GetMessageRegistrations().Count.Should().Be(2);
		}

		[Test]
		public void RegisterPolymorphicHandlerToTheSameCommandIsNotYetInvalid()
		{
			var register = new HandlerRegister()
				.RegisterCommand<CommandB>().To<CommandAHandler>()
				.RegisterCommand<CommandB>().To<CommandBHandler>()
				;

			register.GetMessageRegistrations().Count.Should().Be(2);
		}

		[Test]
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
