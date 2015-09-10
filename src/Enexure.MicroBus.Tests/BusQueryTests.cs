using System;
using System.Threading.Tasks;
using Enexure.MicroBus.Annotations;
using FluentAssertions;
using NUnit.Framework;

namespace Enexure.MicroBus.Tests
{
	[TestFixture]
	public class QueryTests
	{
		class Query : IQuery<Query, Result> { }

		class Result : IResult { }

		[UsedImplicitly]
		class QueryHandler : IQueryHandler<Query, Result>
		{
			public Task<Result> Handle(Query query)
			{
				return Task.FromResult(new Result());
			}
		}

		[Test]
		public void NoHandlerShouldThrow()
		{
			var bus = BusBuilder.BuildBus(b => b);

			var func = (Func<Task>)(() => bus.Query(new Query()));

			func.ShouldThrowExactly<NoRegistrationForMessageException>().WithMessage("No registration for message of type Query was found");
		}


		[Test]
		public async Task TestQuery()
		{
			var bus = BusBuilder.BuildBus(b => 
				b.RegisterQuery<Query, Result>().To<QueryHandler>()
			);

			var result = await bus.Query(new Query());

			result.Should().NotBeNull();

		}
	}
}
