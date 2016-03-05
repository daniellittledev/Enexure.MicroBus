using System;
using System.Threading.Tasks;

namespace Enexure.MicroBus
{
	public interface IQueryHandler<in TQuery, TResult>
		where TQuery : IQuery<TQuery, TResult>
	{
		Task<TResult> Handle(TQuery query);
	}
}
