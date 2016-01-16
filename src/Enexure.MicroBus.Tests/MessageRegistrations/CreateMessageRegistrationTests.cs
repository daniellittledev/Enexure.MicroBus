using System;
using FluentAssertions;
using Xunit;

namespace Enexure.MicroBus.Tests.MessageRegistrations
{
	public class CreateMessageRegistrationTests
	{
		[Fact]
		public void MatchingEventAndHandlerShouldBeAbleToBeCreated()
		{
			new MessageRegistration(typeof(EventA), typeof(EventAHandler));
		}

		[Fact]
		public void SuperTypeEventAndSubTypeHandlerShouldBeAbleToBeCreated()
		{
			new MessageRegistration(typeof(EventB), typeof(EventAHandler));
		}

		[Fact]
		public void ExtraSuperTypeEventAndSubTypeHandlerShouldBeAbleToBeCreated()
		{
			new MessageRegistration(typeof(EventC), typeof(EventAHandler));
		}

		[Fact]
		public void HandlerWhichCannotSupportEventShouldThrow()
		{
			new Action(() => new MessageRegistration(typeof(EventA), typeof(EventBHandler))).ShouldThrow<ArgumentException>();
		}
	}
}