using System.Threading.Tasks;

namespace Enexure.MicroBus
{
	public interface IEventHandler<in TEvent>
		where TEvent : IEvent
	{
		Task Handle(TEvent @event);
	}
}
