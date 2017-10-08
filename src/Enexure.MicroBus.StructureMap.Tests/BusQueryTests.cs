using System.Threading.Tasks;
using StructureMap;
using FluentAssertions;
using Xunit;

namespace Enexure.MicroBus.StructureMap.Tests
{
    public class StructureMapQueryTests
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

            var container = new Container(b => b.RegisterMicroBus(busBuilder));

            var bus = container.GetInstance<IMicroBus>();

            var result = await bus.QueryAsync(new QueryAsync());

            result.Should().NotBeNull();

        }
    }
}
