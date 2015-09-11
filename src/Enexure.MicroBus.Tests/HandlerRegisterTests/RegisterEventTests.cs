using FluentAssertions;
using NUnit.Framework;

namespace Enexure.MicroBus.Tests.HandlerRegisterTests
{
	[TestFixture]
	public class RegisterEventTests
	{
		[Test]
		public void RegisterThreeHandlersToASingleMessage()
		{
			var register = new HandlerRegister()
				.RegisterEvent<EventB>().To(x => x
						.Handler<EventInterfaceHandler>()
						.Handler<EventAHandler>()
						.Handler<EventBHandler>()
				);

			register.GetMessageRegistrations().Count.Should().Be(3);
		}

		[Test]
		public void RegisterThreeHandlersToASingleMessageIndividually()
		{
			var register = new HandlerRegister()
				.RegisterEvent<EventB>().To<EventInterfaceHandler>()
				.RegisterEvent<EventB>().To<EventAHandler>()
				.RegisterEvent<EventB>().To<EventBHandler>()
				;

			register.GetMessageRegistrations().Count.Should().Be(3);
		}

		[Test]
		public void RegisterThreeHandlersToTheirOwnMessages()
		{
			var register = new HandlerRegister()
				.RegisterEvent<IEvent>().To<EventInterfaceHandler>()
				.RegisterEvent<EventA>().To<EventAHandler>()
				.RegisterEvent<EventB>().To<EventBHandler>()
				;

			register.GetMessageRegistrations().Count.Should().Be(3);
		}

		[Test]
		public void MixingIndividuallyAndCombinedHandlerRegistration()
		{
			var register = new HandlerRegister()
				.RegisterEvent<EventA>().To(x => x
					.Handler<EventInterfaceHandler>()
					.Handler<EventAHandler>()
				)
				.RegisterEvent<EventA>().To<OtherEventAHandler>()
				;

			register.GetMessageRegistrations().Count.Should().Be(3);
		}
	}
}
