using System;

namespace Enexure.MicroBus
{
	public class QueryBuilder<TQuery, TResult> : IQueryBuilder<TQuery, TResult>
		where TQuery : IQuery<TQuery, TResult>
		where TResult : IResult
	{
		private readonly BusBuilder busBuilder;

		public QueryBuilder(BusBuilder busBuilder)
		{
			this.busBuilder = busBuilder;
		}

		public IBusBuilder To<TQueryHandler>() where TQueryHandler : IQueryHandler<TQuery, TResult>
		{
			return To<TQueryHandler>(new Pipeline());
		}

		public IBusBuilder To<TQueryHandler>(Pipeline pipeline)
			where TQueryHandler : IQueryHandler<TQuery, TResult>
		{
			if (pipeline == null) throw new ArgumentNullException("pipeline");

			return new BusBuilder(busBuilder, new MessageRegistration(typeof(TQuery), typeof(TQueryHandler), pipeline));
		}
	}

	public class QueryBuilder : IQueryBuilder
	{
		private readonly BusBuilder busBuilder;
		private readonly Type queryType;

		public QueryBuilder(BusBuilder busBuilder, Type queryType)
		{
			this.busBuilder = busBuilder;
			this.queryType = queryType;
		}

		public IBusBuilder To(Type queryHandlerType)
		{
			return To(queryHandlerType, new Pipeline());
		}

		public IBusBuilder To(Type queryHandlerType, Pipeline pipeline)
		{
			return new BusBuilder(busBuilder, new MessageRegistration(queryType, queryHandlerType, pipeline));
		}
	}
}