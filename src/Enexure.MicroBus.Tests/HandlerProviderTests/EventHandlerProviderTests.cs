using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace Enexure.MicroBus.Tests.HandlerProviderTests
{
	[TestFixture]
	public class EventHandlerProviderTests
	{
		[Test]
		public void NoRegistrationShouldBeFine()
		{
			var provider = HandlerProvider.Create(Enumerable.Empty<MessageRegistration>());
		}

		[Test]
		public void RetrievalOfAMessageThatWasNotRegistered()
		{
			var provider = HandlerProvider.Create(Enumerable.Empty<MessageRegistration>());

			GroupedMessageRegistration registration;
			provider.GetRegistrationForMessage(typeof(EventA), out registration);

			registration.Should().BeNull();
		}

		[Test]
		public void BasicRegistrationAndRetrieval()
		{
			var provider = HandlerProvider.Create(new [] {
				new MessageRegistration(typeof(EventA), typeof(EventAHandler), new Pipeline()), 
			});

			GroupedMessageRegistration registration;
			provider.GetRegistrationForMessage(typeof(EventA), out registration);

			registration.Should().NotBeNull();
			registration.Handlers.Count.Should().Be(1);
		}

		[Test]
		public void GroupingRegistrationAndRetrieval()
		{
			var pipeline = new Pipeline();

			var provider = HandlerProvider.Create(new[] {
				new MessageRegistration(typeof(EventA), typeof(EventAHandler), pipeline),
				new MessageRegistration(typeof(EventA), typeof(OtherEventAHandler), pipeline),
			});

			GroupedMessageRegistration registration;
			provider.GetRegistrationForMessage(typeof(EventA), out registration);

			registration.Should().NotBeNull();
			registration.Handlers.Count.Should().Be(2);
		}

		[Test]
		public void PolymorphicRegistrationAndPolymorphicRetrieval()
		{
			var pipeline = new Pipeline();

			var provider = HandlerProvider.Create(new[] {
				new MessageRegistration(typeof(EventA), typeof(EventAHandler), pipeline),
				new MessageRegistration(typeof(EventB), typeof(EventBHandler), pipeline),
			});

			GroupedMessageRegistration registration;
			provider.GetRegistrationForMessage(typeof(EventA), out registration);

			registration.Should().NotBeNull();
			registration.Handlers.Count.Should().Be(1);
		}

		[Test]
		public void PolymorphicRegistrationAndBasicRetrieval()
		{
			var pipeline = new Pipeline();

			var provider = HandlerProvider.Create(new[] {
				new MessageRegistration(typeof(EventA), typeof(EventAHandler), pipeline),
				new MessageRegistration(typeof(EventB), typeof(EventBHandler), pipeline),
			});

			GroupedMessageRegistration registration;
			provider.GetRegistrationForMessage(typeof(EventB), out registration);

			registration.Should().NotBeNull();
			registration.Handlers.Count.Should().Be(2);
		}

		[Test]
		public void OrderOfHandlersShouldStartWithTheLeastSpecificMessageTypeRegistration()
		{
			var pipeline = new Pipeline();

			var provider = HandlerProvider.Create(new[] {
				new MessageRegistration(typeof(EventB), typeof(EventBHandler), pipeline),
				new MessageRegistration(typeof(EventC), typeof(EventCHandler), pipeline),
				new MessageRegistration(typeof(EventA), typeof(EventAHandler), pipeline),
			});

			GroupedMessageRegistration registration;
			provider.GetRegistrationForMessage(typeof(EventC), out registration);

			registration.Handlers.Count.Should().Be(3);
			registration.Handlers.Skip(0).First().Should().Be(typeof(EventAHandler));
			registration.Handlers.Skip(1).First().Should().Be(typeof(EventBHandler));
			registration.Handlers.Skip(2).First().Should().Be(typeof(EventCHandler));
		}

		[Test]
		public void RegisteringTwoDifferentPipelinesShouldThrowAnException()
		{
			new Action(() => { HandlerProvider.Create(new[] {
					new MessageRegistration(typeof(EventA), typeof(EventAHandler), new Pipeline()),
					new MessageRegistration(typeof(EventB), typeof(EventBHandler), new Pipeline()),
				});
			}).ShouldThrowExactly<MultipleDifferentPipelinesRegisteredException>();
			

		}
	}
}
