using FluentAssertions;
using NUnit.Framework;

namespace Enexure.MicroBus.Tests.HandlerRegisterTests
{
	[TestFixture]
	public class RegisterQueryTests
	{
		[Test]
		public void RegisterASingleQueryHandlerShouldReturnOneRegistration()
		{
			var register = new HandlerRegister()
				.RegisterQuery<QueryA, ResultA>().To<QueryAHandler>();

			register.GetMessageRegistrations().Count.Should().Be(1);
		}


		[Test]
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
