using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Enexure.MicroBus.Autofac.Tests
{
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

		class Result { }

		class QueryAsync : IQuery<QueryAsync, Result> { }

		class QueryHandler : IQueryHandler<QueryAsync, Result>
		{
			public Task<Result> Handle(QueryAsync @QueryAsync) { return Task.FromResult(new Result()); }
		}

		[Fact]
		public void ScanAnAssemblyForHandlersTest()
		{
			var register = new BusBuilder().RegisterHandlers(
				typeof(AutofacTypeScannerTests).GetTypeInfo().Assembly,
				x => x.FullName.Contains("AutofacTypeScannerTests"));

			register.MessageHandlerRegistrations.Count.Should().Be(3);

		}
	}
}
