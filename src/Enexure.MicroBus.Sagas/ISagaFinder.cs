using System.Threading.Tasks;

namespace Enexure.MicroBus.Sagas
{
    public interface ISagaFinder<TSaga, in TMessage> 
        where TSaga : class, ISaga
    {
        Task<TSaga> FindByAsync(TMessage message);
    }
}