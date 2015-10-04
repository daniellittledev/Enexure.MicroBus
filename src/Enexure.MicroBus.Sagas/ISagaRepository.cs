using System.Threading.Tasks;

namespace Enexure.MicroBus.Sagas
{
	public interface ISagaRepository
	{
		Task<ISaga>GetSagaForAsync<TSaga, TEvent>(TEvent message)
			where TSaga : ISaga
			where TEvent : IEvent;

		Task CreateAsync(ISaga saga);
		Task UpdateAsync(ISaga saga);
		Task CompleteAsync(ISaga saga);
	}
}