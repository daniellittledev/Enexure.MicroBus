using System;

namespace Enexure.MicroBus
{
	public class QueryBuilder<TQuery, TResult> : IQueryBuilder<TQuery, TResult>
		where TQuery : IQuery<TQuery, TResult>
		where TResult : IResult
	{
		private readonly HandlerRegister handlerRegister;

		public QueryBuilder(HandlerRegister handlerRegister)
		{
			this.handlerRegister = handlerRegister;
		}

		public IHandlerRegister To<TQueryHandler>() where TQueryHandler : IQueryHandler<TQuery, TResult>
		{
			return To<TQueryHandler>(Pipeline.EmptyPipeline);
		}

		public IHandlerRegister To<TQueryHandler>(Pipeline pipeline)
			where TQueryHandler : IQueryHandler<TQuery, TResult>
		{
			if (pipeline == null) throw new ArgumentNullException("pipeline");

			return new HandlerRegister(handlerRegister, new MessageRegistration(typeof(TQuery), typeof(TQueryHandler), pipeline));
		}
	}

	public class QueryBuilder : IQueryBuilder
	{
		private readonly HandlerRegister handlerRegister;
		private readonly Type queryType;

		public QueryBuilder(HandlerRegister handlerRegister, Type queryType)
		{
			this.handlerRegister = handlerRegister;
			this.queryType = queryType;
		}

		public IHandlerRegister To(Type queryHandlerType)
		{
			return To(queryHandlerType, Pipeline.EmptyPipeline);
		}

		public IHandlerRegister To(Type queryHandlerType, Pipeline pipeline)
		{
			return new HandlerRegister(handlerRegister, new MessageRegistration(queryType, queryHandlerType, pipeline));
		}
	}
}