using System;
using System.Threading.Tasks;

namespace Enexure.MicroBus
{
	using System.Threading;

	public interface IQueryHandler<in TQuery, TResult>
		where TQuery : IQuery<TQuery, TResult>
	{
		Task<TResult> Handle(TQuery query);
	}

	public interface ICancelableQueryHandler<in TQuery, TResult>
		where TQuery : IQuery<TQuery, TResult>
	{
		Task<TResult> Handle(TQuery query, CancellationToken cancellation);
	}
}
