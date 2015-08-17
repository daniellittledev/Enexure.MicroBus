namespace Enexure.MicroBus.Sagas
{
	public interface ISagaFinder<TData, in TMessage>
		where TMessage : IMessage
		where TData : class
	{
		ISaga<TData> FindBy(TMessage message);
	}
}