using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace Enexure.MicroBus.Autofac.Tests
{
	[TestFixture]
	public class AutofacTypeScannerTests
	{
		class Event : IEvent { }

		class EventHandler : IEventHandler<Event>
		{
			public Task Handle(Event @event) { return Task.FromResult(0); }
		}

		class Command : ICommand { }

		class CommandHandler : ICommandHandler<Command>
		{
			public Task Handle(Command @event) { return Task.FromResult(0); }
		}

		class Result : IResult { }

		class Query : IQuery<Query, Result> { }

		class QueryHandler : IQueryHandler<Query, Result>
		{
			public Task<Result> Handle(Query @Query) { return Task.FromResult(new Result()); }
		}

		[Test]
		public void ScanAnAssemblyForHandlersTest()
		{
			var register = new HandlerRegister().RegisterTypes(x => 
				x.FullName.Contains("AutofacTypeScannerTests"),
				Pipeline.EmptyPipeline, 
				typeof(AutofacTypeScannerTests).Assembly);

			register.GetMessageRegistrations().Count.Should().Be(3);

		}
	}
}
