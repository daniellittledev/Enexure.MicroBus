using System;
using System.Threading.Tasks;
using Autofac;
using Enexure.MicroBus.Annotations;
using FluentAssertions;
using Xunit;

namespace Enexure.MicroBus.Autofac.Tests
{
	public class ScopesAndDisposeTests
	{
		[UsedImplicitly]
		private class DisposableObject : IDisposable
		{
			public bool IsDisposed { get; set; }

			public void Dispose()
			{
				IsDisposed = true;
			}
		}

		class Command : ICommand { }

		[UsedImplicitly]
		private class CommandHandler : ICommandHandler<Command>
		{
			private readonly DisposableObject disposable;

			public CommandHandler(DisposableObject disposable)
			{
				this.disposable = disposable;
			}

			public async Task Handle(Command command)
			{
				await Task.Delay(1);

				disposable.IsDisposed.Should().BeFalse("");
			}
		}

		class Event : IEvent { }

		[UsedImplicitly]
		private class EventHandler : IEventHandler<Event>
		{
			private readonly DisposableObject disposable;

			public EventHandler(DisposableObject disposable)
			{
				this.disposable = disposable;
			}

			public async Task Handle(Event Event)
			{
				await Task.Delay(1);

				disposable.IsDisposed.Should().BeFalse();
			}
		}

		class Query : IQuery<Query, Result> { }

		private class Result : IResult { }

		[UsedImplicitly]
		private class QueryHandler : IQueryHandler<Query, Result>
		{
			private readonly DisposableObject disposable;

			public QueryHandler(DisposableObject disposable)
			{
				this.disposable = disposable;
			}

			public async Task<Result> Handle(Query Query)
			{
				await Task.Delay(1);

				disposable.IsDisposed.Should().BeFalse();

				return new Result();
			}
		}

		[Fact]
		public async Task InTheDefaultAutofacScopeCommandHandlersShouldFinishBeforeTheScopeIsDisposed()
		{
			var pipeline = Pipeline.EmptyPipeline;

			var containerBuilder = new ContainerBuilder();

			containerBuilder.RegisterType<DisposableObject>().AsSelf();

			var container = containerBuilder.RegisterMicroBus(busBuilder => busBuilder
				.RegisterCommand<Command>().To<CommandHandler>(pipeline)
			).Build();

			var bus = container.Resolve<IMicroBus>();

			await bus.Send(new Command());
		}

		[Fact]
		public async Task InTheDefaultAutofacScopeEventHandlersShouldFinishBeforeTheScopeIsDisposed()
		{
			var pipeline = Pipeline.EmptyPipeline;

			var containerBuilder = new ContainerBuilder();

			containerBuilder.RegisterType<DisposableObject>().AsSelf();

			var container = containerBuilder.RegisterMicroBus(busBuilder => busBuilder
				.RegisterEvent<Event>().To<EventHandler>(pipeline)
			).Build();

			var bus = container.Resolve<IMicroBus>();

			await bus.Publish(new Event());
		}

		[Fact]
		public async Task InTheDefaultAutofacScopeQueryHandlersShouldFinishBeforeTheScopeIsDisposed()
		{
			var pipeline = Pipeline.EmptyPipeline;

			var containerBuilder = new ContainerBuilder();

			containerBuilder.RegisterType<DisposableObject>().AsSelf();

			var container = containerBuilder.RegisterMicroBus(busBuilder => busBuilder
				.RegisterQuery<Query, Result>().To<QueryHandler>(pipeline)
			).Build();

			var bus = container.Resolve<IMicroBus>();

			await bus.Query(new Query());
		}
	}
}
