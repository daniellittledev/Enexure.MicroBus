using System.Threading.Tasks;

namespace Enexure.MicroBus.Sagas
{
	public interface IHandleMessage<in TMessage>
		where TMessage : IMessage
	{
		Task<bool> Handle(TMessage message);
	}
}