using System;

namespace Enexure.MicroBus
{
	public interface IQueryBuilder<out TQuery, TResult>
		where TQuery : IQuery<TQuery, TResult>
		where TResult : IResult
	{
		IMessageRegister To<TQueryHandler>()
			where TQueryHandler : IQueryHandler<TQuery, TResult>;

		IMessageRegister To<TQueryHandler>(Pipeline pipeline)
			where TQueryHandler : IQueryHandler<TQuery, TResult>;
	}

	public interface IQueryBuilder
	{
		IMessageRegister To(Type queryHandlerType);
		IMessageRegister To(Type queryHandlerType, Pipeline pipeline);
	}
}