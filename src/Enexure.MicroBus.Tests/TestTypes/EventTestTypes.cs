using System;
using System.Threading.Tasks;

namespace Enexure.MicroBus.Tests
{
	class EventInterfaceHandler : IEventHandler<IEvent>
	{
		public Task Handle(IEvent @event)
		{
			throw new InvalidOperationException();
		}
	}

	class EventAHandler : IEventHandler<EventA>
	{
		public Task Handle(EventA @event)
		{
			throw new NotSupportedException();
		}
	}

	class OtherEventAHandler : IEventHandler<EventA>
	{
		public Task Handle(EventA @event)
		{
			throw new NotSupportedException();
		}
	}

	class EventBHandler : IEventHandler<EventB>
	{
		public Task Handle(EventB @event)
		{
			throw new NotSupportedException();
		}
	}

	class EventCHandler : IEventHandler<EventC>
	{
		public Task Handle(EventC @event)
		{
			throw new NotSupportedException();
		}
	}

	class EventA : IEvent { }
	class EventB : EventA { }
	class EventC : EventB { }


	class EventX : IEvent { }
	class EventY : EventX { }
	class EventZ : EventY { }
}
