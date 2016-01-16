using System;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Enexure.MicroBus.Tests.HandlerProviderTests
{
	public class QueryHandlerProviderTests
	{
		[Fact]
		public void RetrievalOfAMessageThatWasNotRegistered()
		{
			var provider = HandlerProvider.Create(Enumerable.Empty<MessageRegistration>());

			GroupedMessageRegistration registration;
			provider.GetRegistrationForMessage(typeof(QueryA), out registration);

			registration.Should().BeNull();
		}

		[Fact]
		public void BasicRegistrationAndRetrieval()
		{
			var provider = HandlerProvider.Create(new [] {
				new MessageRegistration(typeof(QueryA), typeof(QueryAHandler), new Pipeline()), 
			});

			GroupedMessageRegistration registration;
			provider.GetRegistrationForMessage(typeof(QueryA), out registration);

			registration.Should().NotBeNull();
			registration.Handlers.Count.Should().Be(1);
		}

		[Fact]
		public void RegisteringTwoQuerysToTheSameMessageShouldFail()
		{
			var pipeline = new Pipeline();

			new Action(() => {
				HandlerProvider.Create(new[] {
					new MessageRegistration(typeof(QueryA), typeof(QueryAHandler), pipeline),
					new MessageRegistration(typeof(QueryA), typeof(OtherQueryAHandler), pipeline),
				});
			}).ShouldThrow<MultipleRegistrationsWithTheSameQueryException>();
		}

	}
}
