using FluentAssertions;
using Xunit;

namespace Enexure.MicroBus.Tests.HandlerRegisterTests
{
	public class RegisterQueryTests
	{
		[Fact]
		public void RegisterASingleQueryHandlerShouldReturnOneRegistration()
		{
			var register = new HandlerRegister()
				.RegisterQuery<QueryA, ResultA>().To<QueryAHandler>();

			register.GetMessageRegistrations().Count.Should().Be(1);
		}


		[Fact]
		public void RegisterTwoHandlersToTheSameQueryIsNotYetInvalid()
		{
			var register = new HandlerRegister()
				.RegisterQuery<QueryA, ResultA>().To<QueryAHandler>()
				.RegisterQuery<QueryA, ResultA>().To<OtherQueryAHandler>()
				;

			register.GetMessageRegistrations().Count.Should().Be(2);
		}
	}
}
