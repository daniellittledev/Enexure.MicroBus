using System.Threading.Tasks;
using Autofac;
using FluentAssertions;
using Xunit;

namespace Enexure.MicroBus.Autofac.Tests
{
	public class AutofacQueryTests
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

			var container = new ContainerBuilder()
				.RegisterMicroBus(busBuilder)
				.Build();

			var bus = container.Resolve<IMicroBus>();

			var result = await bus.QueryAsync(new QueryAsync());

			result.Should().NotBeNull();

		}
	}
}
