using System.Threading.Tasks;

namespace Enexure.MicroBus
{
	using System.Threading;

	public interface IEventHandler<in TEvent>
		where TEvent : IEvent
	{
		Task Handle(TEvent @event);
	}

	public interface ICancelableEventHandler<in TEvent>
		where TEvent : IEvent
	{
		Task Handle(TEvent @event, CancellationToken cancellation);
	}
}
