using System.Linq;
using FluentAssertions;
using Xunit;

namespace Enexure.MicroBus.Tests.HandlerRegistrations
{
	public class MessageExpandingTests
	{
		[Fact]
		public void ExpandingAnInterface()
		{
			var types = ReflectionExtensions.ExpandType(typeof(IEvent)).ToList();
			types.Count.Should().Be(2);
			types[0].Should().Be(typeof(IEvent));
			types[1].Should().Be(typeof(IMessage));
		}

		[Fact]
		public void ExpandingAnEventWithASubType()
		{
			var types = ReflectionExtensions.ExpandType(typeof(MessageC)).ToList();
			types.Count.Should().Be(4);
			types[0].Should().Be(typeof(MessageC));
			types[1].Should().Be(typeof(MessageB));
			types[2].Should().Be(typeof(MessageA));
			types[3].Should().Be(typeof(object));
		}

		class MessageA { }
		class MessageB : MessageA { }
		class MessageC : MessageB { }
	}
}
