using System;
using System.Linq;
using FluentAssertions;
using Xunit;
using System.Threading.Tasks;

namespace Enexure.MicroBus.Tests.HandlerProviderTests
{
	public class QueryHandlerProviderTests
	{
		[Fact]
		public void RegisteringTwoQueriesToTheSameMessageShouldFail()
		{
			new Action(() =>
			{

				var busBuilder = new BusBuilder()
				.RegisterQueryHandler<QueryA, object, QueryAHandler>()
				.RegisterQueryHandler<QueryA, object, OtherQueryAHandler>();

				var piplineBuilder = new PipelineBuilder(busBuilder);
				piplineBuilder.Validate();

			}).ShouldThrow<InvalidDuplicateRegistrationsException>();
		}

		public class QueryA : IQuery<QueryA, object> { }
		class QueryAHandler : IQueryHandler<QueryA, object>
		{
			public Task<object> Handle(QueryA Command)
			{
				throw new NotSupportedException();
			}
		}
		class OtherQueryAHandler : QueryAHandler { }
	}
}
