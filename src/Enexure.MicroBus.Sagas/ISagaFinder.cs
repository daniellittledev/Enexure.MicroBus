using System.Threading.Tasks;

namespace Enexure.MicroBus.Sagas
{
	public interface ISagaFinder<TSaga, in TMessage>
		where TMessage : IMessage
		where TSaga : ISaga
	{
		Task<TSaga> FindByAsync(TMessage message);
	}
}