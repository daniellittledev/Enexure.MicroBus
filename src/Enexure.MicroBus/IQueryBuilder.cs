using System;

namespace Enexure.MicroBus
{
	public interface IQueryBuilder<out TQuery, TResult>
		where TQuery : IQuery<TQuery, TResult>
		where TResult : IResult
	{
		IBusBuilder To<TQueryHandler>()
			where TQueryHandler : IQueryHandler<TQuery, TResult>;

		IBusBuilder To<TQueryHandler>(Pipeline pipeline)
			where TQueryHandler : IQueryHandler<TQuery, TResult>;
	}

	public interface IQueryBuilder
	{
		IBusBuilder To(Type queryHandlerType);
		IBusBuilder To(Type queryHandlerType, Pipeline pipeline);
	}
}