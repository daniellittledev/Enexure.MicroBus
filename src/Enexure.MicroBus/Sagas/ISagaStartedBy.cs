namespace Enexure.MicroBus.Sagas
{
	public interface ISagaStartedBy<in TMessage> : IHandleMessage<TMessage>
		where TMessage : IMessage
	{
	}
}