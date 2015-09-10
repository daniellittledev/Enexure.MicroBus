using NUnit.Framework;
using System.Threading.Tasks;
using System;

namespace Enexure.MicroBus.Tests.BusBuilderTests
{
    [TestFixture]
    public class RegisterEventTests
    {
        class EventBase : IEvent { }

        class EventSuperA : IEvent { }


        class InterfaceEventHandler : IEventHandler<IEvent>
        {
            public Task Handle(IEvent @event)
            {
                return Task.FromResult(0);
            }
        }

        class EventBaseHandler : IEventHandler<EventBase>
        {
            public Task Handle(EventBase @event)
            {
                return Task.FromResult(0);
            }
        }

        class EventSuperAHandler : IEventHandler<EventSuperA>
        {
            public Task Handle(EventSuperA @event)
            {
                return Task.FromResult(0);
            }
        }

        [Test]
        public void PolymorphicDispatch()
        {

            var bus = BusBuilder.BuildBus(b => b
                .RegisterEvent<IEvent>().To<InterfaceEventHandler>()
                .RegisterEvent<EventBase>().To<EventBaseHandler>()
                .RegisterEvent<EventSuperA>().To<EventSuperAHandler>()
            );
        }
    }
}
