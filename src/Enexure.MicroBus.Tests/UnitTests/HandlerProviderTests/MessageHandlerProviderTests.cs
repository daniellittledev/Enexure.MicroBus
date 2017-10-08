using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Enexure.MicroBus.Tests.UnitTests.HandlerProviderTests
{
    public class MessageHandlerProviderTests
    {
        [Fact]
        public void GetPipelineForUnregisteredMessageShouldReturnNull()
        {
            var busBuilder = new BusBuilder();
            var piplineBuilder = new PipelineBuilder(busBuilder);
            var pipeline = piplineBuilder.GetPipeline(typeof(TestMessageA));

            pipeline.Should().NotBeNull();
            pipeline.HandlerTypes.Count.Should().Be(0);
        }

        [Fact]
        public void GetPipelineForRegisteredMessageShouldNotBeNull()
        {
            var busBuilder = new BusBuilder()
                .RegisterHandler<TestMessageA, TestMessageAHandler>();

            var piplineBuilder = new PipelineBuilder(busBuilder);
            var pipeline = piplineBuilder.GetPipeline(typeof(TestMessageA));

            pipeline.Should().NotBeNull();
            pipeline.HandlerTypes.Count.Should().Be(1);
        }

        [Fact]
        public void GetPipelineWithTwoOfTheSameMessageShouldHaveMultipleHandlers()
        {
            var busBuilder = new BusBuilder()
                .RegisterHandler<TestMessageA, TestMessageAHandler>()
                .RegisterHandler<TestMessageA, OtherTestMessageAHandler>();

            var piplineBuilder = new PipelineBuilder(busBuilder);
            var pipeline = piplineBuilder.GetPipeline(typeof(TestMessageA));

            pipeline.Should().NotBeNull();
            pipeline.HandlerTypes.Count.Should().Be(2);
        }

        [Fact]
        public void GetPipelineWithPolymorphicHandlerRegistrationShouldHaveHandlerWithLessSpecializedType()
        {
            var busBuilder = new BusBuilder()
                .RegisterHandler<TestMessageA, TestMessageAHandler>();

            var piplineBuilder = new PipelineBuilder(busBuilder);
            var pipeline = piplineBuilder.GetPipeline(typeof(TestMessageB));

            pipeline.Should().NotBeNull();
            pipeline.HandlerTypes.Count.Should().Be(1);
        }

        [Fact]
        public void MultipleRegistrationsToHandlerShouldOnlyHaveOneHandlerInPipeline()
        {
            var busBuilder = new BusBuilder()
                .RegisterHandler<TestMessageA, TestMessageAHandler>()
                .RegisterHandler<TestMessageB, TestMessageAHandler>()
                .RegisterHandler<TestMessageC, TestMessageAHandler>();

            var piplineBuilder = new PipelineBuilder(busBuilder);
            var pipeline = piplineBuilder.GetPipeline(typeof(TestMessageA));

            pipeline.HandlerTypes.Count.Should().Be(1);
            pipeline.HandlerTypes.First().Should().Be(typeof(TestMessageAHandler));
        }

        #region TypesUsedForTesting
        class TestMessageA { }
        class TestMessageB : TestMessageA { }
        class TestMessageC : TestMessageB { }
        class TestMessageAHandler : IMessageHandler<TestMessageA, Unit>
        {
            public Task<Unit> Handle(TestMessageA message) { return Task.FromResult(Unit.Unit); }
        }
        class OtherTestMessageAHandler : IMessageHandler<TestMessageA, Unit>
        {
            public Task<Unit> Handle(TestMessageA message) { return Task.FromResult(Unit.Unit); }
        }
        class TestMessageBHandler : IMessageHandler<TestMessageB, Unit>
        {
            public Task<Unit> Handle(TestMessageB message) { return Task.FromResult(Unit.Unit); }
        }
        class TestMessageCHandler : IMessageHandler<TestMessageC, Unit>
        {
            public Task<Unit> Handle(TestMessageC message) { return Task.FromResult(Unit.Unit); }
        }
        #endregion
    }
}
