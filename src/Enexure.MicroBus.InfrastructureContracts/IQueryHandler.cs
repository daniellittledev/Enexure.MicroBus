using System;
using System.Threading.Tasks;

namespace Enexure.MicroBus
{
	public interface IQueryHandler<in TQuery, TResult>
		where TQuery : IQuery<TQuery, TResult>
		where TResult : IResult
	{
		Task<TResult> Handle(TQuery query);
	}
}
