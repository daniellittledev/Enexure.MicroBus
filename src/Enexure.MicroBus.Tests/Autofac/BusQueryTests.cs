using System.Threading.Tasks;
using Autofac;
using Enexure.MicroBus.Autofac;
using Enexure.MicroBus.Tests.Common;
using FluentAssertions;
using NUnit.Framework;

namespace Enexure.MicroBus.Tests.Autofac
{
	[TestFixture]
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

		[Test]
		public async Task TestQuery()
		{
			var pipline = new Pipeline()
				.AddHandler<Common.PipelineHandler>();

			var container = new ContainerBuilder().RegisterMicroBus(busBuilder => {

				return busBuilder
					.RegisterQuery<Query, Result>().To<QueryHandler>(pipline);

			}).Build();

			var bus = container.Resolve<IMicroBus>();

			var result = await bus.Query(new Query());

			result.Should().NotBeNull();

		}
	}
}
