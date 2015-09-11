using System;

namespace Enexure.MicroBus
{
	public interface IQueryBuilder<out TQuery, TResult>
		where TQuery : IQuery<TQuery, TResult>
		where TResult : IResult
	{
		IHandlerRegister To<TQueryHandler>()
			where TQueryHandler : IQueryHandler<TQuery, TResult>;

		IHandlerRegister To<TQueryHandler>(Pipeline pipeline)
			where TQueryHandler : IQueryHandler<TQuery, TResult>;
	}

	public interface IQueryBuilder
	{
		IHandlerRegister To(Type queryHandlerType);
		IHandlerRegister To(Type queryHandlerType, Pipeline pipeline);
	}
}