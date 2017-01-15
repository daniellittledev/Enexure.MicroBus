using System;
using FluentAssertions;
using Xunit;

namespace Enexure.MicroBus.Tests.UnitTests.MessageRegistrations
{
	public class CreateHandlerRegistrationTests
	{
		[Fact]
		public void MatchingMessageAndHandlerShouldBeAbleToBeCreated()
		{
			new HandlerRegistration(typeof(MessageA), typeof(MessageAHandler));
		}

		[Fact]
		public void SuperTypeMessageAndSubTypeHandlerShouldBeAbleToBeCreated()
		{
			new HandlerRegistration(typeof(MessageB), typeof(MessageAHandler));
		}

		[Fact]
		public void HandlerWhichCannotSupportEventShouldThrow()
		{
			new Action(() => new HandlerRegistration(typeof(MessageA), typeof(MessageXHandler))).ShouldThrow<ArgumentException>();
		}

		class MessageA { }
		class MessageB : MessageA { }
		class MessageX { }
		class MessageAHandler : MessageHandler<MessageA> { }
		class MessageXHandler : MessageHandler<MessageX> { }
	}
}