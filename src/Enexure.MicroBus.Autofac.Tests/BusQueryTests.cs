using System.Threading.Tasks;
using Autofac;
using FluentAssertions;
using Xunit;

namespace Enexure.MicroBus.Autofac.Tests
{
	public class AutofacQueryTests
	{
		class Query : IQuery<Query, Result> { }

		class Result : IResult { }

		class QueryHandler : IQueryHandler<Query, Result>
		{
			public Task<Result> Handle(Query query)
			{
				return Task.FromResult(new Result());
			}
		}

		[Fact]
		public async Task TestQuery()
		{
			var container = new ContainerBuilder().RegisterMicroBus(busBuilder => {

				return busBuilder
					.RegisterQuery<Query, Result>().To<QueryHandler>();

			}).Build();

			var bus = container.Resolve<IMicroBus>();

			var result = await bus.Query(new Query());

			result.Should().NotBeNull();

		}
	}
}
