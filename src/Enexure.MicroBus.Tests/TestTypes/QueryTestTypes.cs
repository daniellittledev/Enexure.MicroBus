using System;
using System.Threading.Tasks;

namespace Enexure.MicroBus.Tests
{
	class QueryAHandler : IQueryHandler<QueryA, ResultA>
	{
		public Task<ResultA> Handle(QueryA Query)
		{
			throw new NotSupportedException();
		}
	}

	class OtherQueryAHandler : IQueryHandler<QueryA, ResultA>
	{
		public Task<ResultA> Handle(QueryA Query)
		{
			throw new NotSupportedException();
		}
	}

	class QueryA : IQuery<QueryA, ResultA> { }

	class ResultA : IResult { }

	class QueryX : IQuery<QueryX, ResultX> { }

	class ResultX : IResult { }
}
