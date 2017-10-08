using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Enexure.MicroBus.Sagas
{
    public interface ISagaRepository<TSaga>
        where TSaga : class, ISaga
    {
        Task CreateAsync(TSaga saga);
        Task UpdateAsync(TSaga saga);
        Task CompleteAsync(TSaga saga);

        TSaga NewSaga();
        Task<TSaga> FindAsync(IEvent message);
    }
}