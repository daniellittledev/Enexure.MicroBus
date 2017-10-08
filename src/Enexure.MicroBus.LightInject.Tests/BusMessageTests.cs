using LightInject;
using System.Threading.Tasks;
using Xunit;
using System.Reflection;
using System.Collections.Generic;

namespace Enexure.MicroBus.LightInject.Tests
{
    public class BusMessageTests
    {
        [Fact]
        public async Task FullSystemTest()
        {
            var assembly = GetType().GetTypeInfo().Assembly;
            var busBuilder = new BusBuilder()
                .RegisterGlobalHandler<OuterHandler>()
                .RegisterGlobalHandler<InnerHandler>()
                .RegisterHandlers(x => x.FullName.Contains("BusMessageTests"), assembly);

            var container = new ServiceContainer();
            container.RegisterMicroBus(busBuilder);

            var mediator = container.GetInstance<IMicroMediator>();

            await mediator.SendAsync(new Message());
        }

        class Message
        {
            Queue<string> queue = new Queue<string>(new[] {
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
                (message as Message).AssertStage("Outer-In");

                var result = await next.Handle(message);

                (message as Message).AssertStage("Outer-Out");

                return result;
            }
        }

        class InnerHandler : IDelegatingHandler
        {
            public async Task<object> Handle(INextHandler next, object message)
            {
                (message as Message).AssertStage("Inner-In");

                var result = await next.Handle(message);

                (message as Message).AssertStage("Inner-Out");

                return result;
            }
        }
    }
}
