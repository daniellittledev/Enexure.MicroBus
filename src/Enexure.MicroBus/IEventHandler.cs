using System;
using System.Threading.Tasks;
using Enexure.MicroBus.MessageContracts;

namespace Enexure.MicroBus
{
	public interface IEventHandler<in TEvent>
		where TEvent : IEvent
	{
		Task Handle(TEvent @event);
	}
}
