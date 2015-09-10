using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace Enexure.MicroBus.Tests.HandlerProviderTests
{
	[TestFixture]
	public class PolymorphicRegistrationTests
	{
		private class MessageAHandler : IEventHandler<MessageA>
		{
			public Task Handle(MessageA @event)
			{
				throw new NotImplementedException();
			}
		}

		private class OtherMessageAHandler : IEventHandler<MessageA>
		{
			public Task Handle(MessageA @event)
			{
				throw new NotImplementedException();
			}
		}

		private class MessageBHandler : IEventHandler<MessageB>
		{
			public Task Handle(MessageB @event)
			{
				throw new NotImplementedException();
			}
		}

		private class MessageCHandler : IEventHandler<MessageC>
		{
			public Task Handle(MessageC @event)
			{
				throw new NotImplementedException();
			}
		}

		private class MessageA : IEvent { }
		private class MessageB : MessageA { }
		private class MessageC : MessageB { }


		private class MessageX : IEvent { }
		private class MessageY : MessageX { }
		private class MessageZ : MessageY { }

		[Test]
		public void NoRegistrationShouldBeFine()
		{
			var provider = new HandlerProvider(Enumerable.Empty<MessageRegistration>());
		}

		[Test]
		public void RetrievalOfAMessageThatWasNotRegistered()
		{
			var provider = new HandlerProvider(Enumerable.Empty<MessageRegistration>());

			GroupedMessageRegistration registration;
			provider.GetRegistrationForMessage(typeof(MessageA), out registration);

			registration.Should().BeNull();
		}

		[Test]
		public void BasicRegistrationAndRetrieval()
		{
			var provider = new HandlerProvider(new [] {
				new MessageRegistration(typeof(MessageA), typeof(MessageAHandler), new Pipeline()), 
			});

			GroupedMessageRegistration registration;
			provider.GetRegistrationForMessage(typeof(MessageA), out registration);

			registration.Should().NotBeNull();
			registration.Handlers.Count.Should().Be(1);
		}

		[Test]
		public void GroupingRegistrationAndRetrieval()
		{
			var pipeline = new Pipeline();

			var provider = new HandlerProvider(new[] {
				new MessageRegistration(typeof(MessageA), typeof(MessageAHandler), pipeline),
				new MessageRegistration(typeof(MessageA), typeof(OtherMessageAHandler), pipeline),
			});

			GroupedMessageRegistration registration;
			provider.GetRegistrationForMessage(typeof(MessageA), out registration);

			registration.Should().NotBeNull();
			registration.Handlers.Count.Should().Be(2);
		}

		[Test]
		public void PolymorphicRegistrationAndPolymorphicRetrieval()
		{
			var pipeline = new Pipeline();

			var provider = new HandlerProvider(new[] {
				new MessageRegistration(typeof(MessageA), typeof(MessageAHandler), pipeline),
				new MessageRegistration(typeof(MessageB), typeof(MessageBHandler), pipeline),
			});

			GroupedMessageRegistration registration;
			provider.GetRegistrationForMessage(typeof(MessageA), out registration);

			registration.Should().NotBeNull();
			registration.Handlers.Count.Should().Be(1);
		}

		[Test]
		public void PolymorphicRegistrationAndBasicRetrieval()
		{
			var pipeline = new Pipeline();

			var provider = new HandlerProvider(new[] {
				new MessageRegistration(typeof(MessageA), typeof(MessageAHandler), pipeline),
				new MessageRegistration(typeof(MessageB), typeof(MessageBHandler), pipeline),
			});

			GroupedMessageRegistration registration;
			provider.GetRegistrationForMessage(typeof(MessageB), out registration);

			registration.Should().NotBeNull();
			registration.Handlers.Count.Should().Be(2);
		}

		[Test]
		public void OrderOfHandlersShouldStartWithTheLeastSpecificMessageTypeRegistration()
		{
			var pipeline = new Pipeline();

			var provider = new HandlerProvider(new[] {
				new MessageRegistration(typeof(MessageB), typeof(MessageBHandler), pipeline),
				new MessageRegistration(typeof(MessageC), typeof(MessageCHandler), pipeline),
				new MessageRegistration(typeof(MessageA), typeof(MessageAHandler), pipeline),
			});

			GroupedMessageRegistration registration;
			provider.GetRegistrationForMessage(typeof(MessageC), out registration);

			registration.Handlers.Count.Should().Be(3);
			registration.Handlers.Skip(0).First().Should().Be(typeof(MessageAHandler));
			registration.Handlers.Skip(1).First().Should().Be(typeof(MessageBHandler));
			registration.Handlers.Skip(2).First().Should().Be(typeof(MessageCHandler));
		}

		[Test]
		public void RegisteringTwoDifferentPipelinesShouldThrowAnException()
		{
			new Action(() => { new HandlerProvider(new[] {
					new MessageRegistration(typeof(MessageA), typeof(MessageAHandler), new Pipeline()),
					new MessageRegistration(typeof(MessageB), typeof(MessageBHandler), new Pipeline()),
				});
			}).ShouldThrowExactly<MultipleDifferentPipelinesRegisteredException>();
			

		}
	}
}
