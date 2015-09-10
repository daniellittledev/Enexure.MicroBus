using System;

namespace Enexure.MicroBus
{
	public class QueryBuilder<TQuery, TResult> : IQueryBuilder<TQuery, TResult>
		where TQuery : IQuery<TQuery, TResult>
		where TResult : IResult
	{
		private readonly HandlerRegister MessageRegister;

		public QueryBuilder(HandlerRegister MessageRegister)
		{
			this.MessageRegister = MessageRegister;
		}

		public IMessageRegister To<TQueryHandler>() where TQueryHandler : IQueryHandler<TQuery, TResult>
		{
			return To<TQueryHandler>(Pipeline.EmptyPipeline);
		}

		public IMessageRegister To<TQueryHandler>(Pipeline pipeline)
			where TQueryHandler : IQueryHandler<TQuery, TResult>
		{
			if (pipeline == null) throw new ArgumentNullException("pipeline");

			return new HandlerRegister(MessageRegister, new MessageRegistration(typeof(TQuery), typeof(TQueryHandler), pipeline));
		}
	}

	public class QueryBuilder : IQueryBuilder
	{
		private readonly HandlerRegister MessageRegister;
		private readonly Type queryType;

		public QueryBuilder(HandlerRegister MessageRegister, Type queryType)
		{
			this.MessageRegister = MessageRegister;
			this.queryType = queryType;
		}

		public IMessageRegister To(Type queryHandlerType)
		{
			return To(queryHandlerType, Pipeline.EmptyPipeline);
		}

		public IMessageRegister To(Type queryHandlerType, Pipeline pipeline)
		{
			return new HandlerRegister(MessageRegister, new MessageRegistration(queryType, queryHandlerType, pipeline));
		}
	}
}