namespace Enexure.MicroBus.Sagas
{
	public interface ISagaStartedBy<in TEvent> : IEventHandler<TEvent>
		where TEvent : IEvent
	{
	}
}