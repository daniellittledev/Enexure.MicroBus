using Autofac;
using System.Threading.Tasks;
using Xunit;
using System.Reflection;
using System.Collections.Generic;

namespace Enexure.MicroBus.Autofac.Tests
{
    public class BusMessageTests
    {
        [Fact]
        public async Task FullSystemTest()
        {
            var assembly = GetType().GetTypeInfo().Assembly;
            var containerBuilder = new ContainerBuilder();
            var busBuilder = new BusBuilder()
                .RegisterGlobalHandler<OuterHandler>()
                .RegisterGlobalHandler<InnerHandler>()
                .RegisterHandlers(x => x.FullName.Contains("BusMessageTests"), assembly);

            containerBuilder.RegisterMicroBus(busBuilder);

            var container = containerBuilder.Build();

            var mediator = container.Resolve<IMicroMediator>();

            await mediator.SendAsync(new Message());
        }

        class Message
        {
            readonly Queue<string> queue = new Queue<string>(new[] {
                "Outer-In",
                "Inner-In",
                "Handler",
                "Inner-Out",
                "Outer-Out",
            });

            public void AssertStage(string stageName)
            {
                Assert.Equal(queue.Dequeue(), stageName);
            }
        }

        class Handler : IMessageHandler<Message, Unit>
        {
            public Task<Unit> Handle(Message message)
            {
                message.AssertStage("Handler");

                return Task.FromResult(Unit.Unit);
            }
        }

        class OuterHandler : IDelegatingHandler
        {
            public async Task<object> Handle(INextHandler next, object message)
            {
                ((Message)message).AssertStage("Outer-In");

                var result = await next.Handle(message);

                ((Message)message).AssertStage("Outer-Out");

                return result;
            }
        }

        class InnerHandler : IDelegatingHandler
        {
            public async Task<object> Handle(INextHandler next, object message)
            {
                ((Message)message).AssertStage("Inner-In");

                var result = await next.Handle(message);

                ((Message)message).AssertStage("Inner-Out");

                return result;
            }
        }
    }
}
