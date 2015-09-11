using System;
using System.Threading.Tasks;

namespace Enexure.MicroBus.Tests
{
	class EventInterfaceHandler : IEventHandler<IEvent>
	{
		public Task Handle(IEvent @event)
		{
			throw new NotImplementedException();
		}
	}

	class EventAHandler : IEventHandler<EventA>
	{
		public Task Handle(EventA @event)
		{
			throw new NotImplementedException();
		}
	}

	class OtherEventAHandler : IEventHandler<EventA>
	{
		public Task Handle(EventA @event)
		{
			throw new NotImplementedException();
		}
	}

	class EventBHandler : IEventHandler<EventB>
	{
		public Task Handle(EventB @event)
		{
			throw new NotImplementedException();
		}
	}

	class EventCHandler : IEventHandler<EventC>
	{
		public Task Handle(EventC @event)
		{
			throw new NotImplementedException();
		}
	}

	class EventA : IEvent { }
	class EventB : EventA { }
	class EventC : EventB { }


	class EventX : IEvent { }
	class EventY : EventX { }
	class EventZ : EventY { }
}
