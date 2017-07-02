using System.Threading.Tasks;
using LightInject;
using FluentAssertions;
using Xunit;

namespace Enexure.MicroBus.LightInject.Tests
{
	public class LightInjectQueryTests
	{
		class QueryAsync : IQuery<QueryAsync, Result> { }

		class Result { }

		class QueryHandler : IQueryHandler<QueryAsync, Result>
		{
			public Task<Result> Handle(QueryAsync query)
			{
				return Task.FromResult(new Result());
			}
		}

		[Fact]
		public async Task TestQuery()
		{
			var busBuilder = new BusBuilder()
				.RegisterQueryHandler<QueryAsync, Result, QueryHandler>();

		    var container = new ServiceContainer();
		    container.RegisterMicroBus(busBuilder);

            var bus = container.GetInstance<IMicroBus>();

			var result = await bus.QueryAsync(new QueryAsync());

			result.Should().NotBeNull();

		}
	}
}
