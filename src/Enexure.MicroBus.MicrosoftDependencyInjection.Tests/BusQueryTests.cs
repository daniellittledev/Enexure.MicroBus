using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using Xunit;

namespace Enexure.MicroBus.MicrosoftDependencyInjection.Tests
{
    public class MicrosoftDependencyInjectionQueryTests
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

            var container = new ServiceCollection()
                .RegisterMicroBus(busBuilder)
                .BuildServiceProvider();

            var bus = container.GetRequiredService<IMicroBus>();

            var result = await bus.QueryAsync(new QueryAsync());

            result.Should().NotBeNull();

        }
    }
}
