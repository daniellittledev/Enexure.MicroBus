using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Enexure.MicroBus.Tests.HandlerProviderTests
{
	[TestFixture]
	public class QueryHandlerProviderTests
	{
		[Test]
		public void RetrievalOfAMessageThatWasNotRegistered()
		{
			var provider = HandlerProvider.Create(Enumerable.Empty<MessageRegistration>());

			GroupedMessageRegistration registration;
			provider.GetRegistrationForMessage(typeof(QueryA), out registration);

			registration.Should().BeNull();
		}

		[Test]
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

		[Test]
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
