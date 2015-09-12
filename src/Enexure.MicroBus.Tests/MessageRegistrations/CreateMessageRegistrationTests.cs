using System;
using FluentAssertions;
using NUnit.Framework;

namespace Enexure.MicroBus.Tests.MessageRegistrations
{
	[TestFixture]
	public class CreateMessageRegistrationTests
	{
		[Test]
		public void MatchingEventAndHandlerShouldBeAbleToBeCreated()
		{
			new MessageRegistration(typeof(EventA), typeof(EventAHandler));
		}

		[Test]
		public void SuperTypeEventAndSubTypeHandlerShouldBeAbleToBeCreated()
		{
			new MessageRegistration(typeof(EventB), typeof(EventAHandler));
		}

		[Test]
		public void ExtraSuperTypeEventAndSubTypeHandlerShouldBeAbleToBeCreated()
		{
			new MessageRegistration(typeof(EventC), typeof(EventAHandler));
		}

		[Test]
		public void HandlerWhichCannotSupportEventShouldThrow()
		{
			new Action(() => new MessageRegistration(typeof(EventA), typeof(EventBHandler))).ShouldThrow<ArgumentException>();
		}
	}
}