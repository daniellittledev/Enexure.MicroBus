using System.Linq;
using FluentAssertions;
using Xunit;

namespace Enexure.MicroBus.Tests.MessageRegistrations
{
	public class MessageExpandingTests
	{
		[Fact]
		public void ExpandingAnInterface()
		{
			var types = MessagesHelper.ExpandType(typeof(IEvent)).ToList();
			types.Count.Should().Be(1);
			types.First().Should().Be(typeof(IEvent));
		}

		[Fact]
		public void ExpandingAnEventWithASubType()
		{
			var types = MessagesHelper.ExpandType(typeof(EventB)).ToList();
			types.Count.Should().Be(3);
			types.Skip(0).First().Should().Be(typeof(IEvent));
			types.Skip(1).First().Should().Be(typeof(EventA));
			types.Skip(2).First().Should().Be(typeof(EventB));
		}
	}
}
