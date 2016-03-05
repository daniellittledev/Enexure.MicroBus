using System.Threading.Tasks;

namespace Enexure.MicroBus.Sagas
{
	public interface ISagaFinder<TSaga, in TEvent>
		where TEvent : IEvent
		where TSaga : class, ISaga
	{
		Task<TSaga> FindByAsync(TEvent message);
	}
}