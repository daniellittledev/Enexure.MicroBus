using System;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Enexure.MicroBus.Tests.HandlerProviderTests
{
	public class CommandHandlerProviderTests
	{
		[Fact]
		public void RetrievalOfAMessageThatWasNotRegistered()
		{
			var provider = HandlerProvider.Create(Enumerable.Empty<MessageRegistration>());

			GroupedMessageRegistration registration;
			provider.GetRegistrationForMessage(typeof(CommandA), out registration);

			registration.Should().BeNull();
		}

		[Fact]
		public void BasicRegistrationAndRetrieval()
		{
			var provider = HandlerProvider.Create(new [] {
				new MessageRegistration(typeof(CommandA), typeof(CommandAHandler), new Pipeline()), 
			});

			GroupedMessageRegistration registration;
			provider.GetRegistrationForMessage(typeof(CommandA), out registration);

			registration.Should().NotBeNull();
			registration.Handlers.Count.Should().Be(1);
		}

		[Fact]
		public void RegisteringTwoCommandsToTheSameMessageShouldFail()
		{
			var pipeline = new Pipeline();

			new Action(() => {
				HandlerProvider.Create(new[] {
					new MessageRegistration(typeof(CommandA), typeof(CommandAHandler), pipeline),
					new MessageRegistration(typeof(CommandA), typeof(OtherCommandAHandler), pipeline),
				});
			}).ShouldThrow<MultipleRegistrationsWithTheSameCommandException>();
		}

	}
}
