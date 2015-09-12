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
		[UsedImplicitly]
		class QueryHandler : IQueryHandler<QueryA, ResultA>
		{
			public Task<ResultA> Handle(QueryA Query)
			{
				return Task.FromResult(new ResultA());
			}
		}

		[Test]
		public void NoHandlerShouldThrow()
		{
			var bus = BusBuilder.BuildBus(b => b);

			var func = (Func<Task>)(() => bus.Query(new QueryA()));

			func.ShouldThrowExactly<NoRegistrationForMessageException>().WithMessage($"No registration for message of type {typeof(QueryA).Name} was found");
		}


		[Test]
		public async Task TestQuery()
		{
			var bus = BusBuilder.BuildBus(b => 
				b.RegisterQuery<QueryA, ResultA>().To<QueryHandler>()
			);

			var result = await bus.Query(new QueryA());

			result.Should().NotBeNull();

		}
	}
}
