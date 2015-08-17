namespace Enexure.MicroBus
{
	public interface IEventBinder<out TEvent> where TEvent : IEvent
	{
		IEventBinder<TEvent> Handler<THandler>()
			where THandler : IEventHandler<TEvent>;
	}
}