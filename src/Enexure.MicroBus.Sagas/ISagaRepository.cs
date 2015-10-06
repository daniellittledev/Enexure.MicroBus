using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Enexure.MicroBus.Sagas
{
	public interface ISagaRepository
	{
		Task<ISaga> GetAsync(Guid sagaId);
		Task<ISaga> FindAsync(Expression<Func<ISaga, bool>> predicate);

		Task CreateAsync(ISaga saga);
		Task UpdateAsync(ISaga saga);
		Task CompleteAsync(ISaga saga);
	}
}