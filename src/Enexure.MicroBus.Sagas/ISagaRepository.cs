using System.Threading.Tasks;

namespace Enexure.MicroBus.Sagas
{
	public interface ISagaRepository
	{
		Task<ISaga> GetSagaForAsync(IMessage message);
		Task UpdateAsync(ISaga saga);
		Task CompleteAsync(ISaga saga);
	}
}