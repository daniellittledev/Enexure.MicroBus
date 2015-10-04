using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Enexure.MicroBus.Sagas
{
	public interface ISagaStore : IEnumerable<ISaga>
	{
		void Add(ISaga saga);
		void Remove(ISaga saga);
		ISaga Get(Guid sagaId);
		ISaga Find(Expression<Func<ISaga, bool>> predicate);
	}
}