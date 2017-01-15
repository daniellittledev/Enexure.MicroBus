using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Xunit;
using Enexure.MicroBus.Autofac;
using FluentAssertions;

namespace Enexure.MicroBus.Tests.IntegrationTests.Cancellation
{
	public class CancellationTests
	{
		public enum CancelStage
		{
			NotCancelled,
			DelegatingHandler,
			MessageHandler
		}

		private ICancelableMicroBus GetBus(BusBuilder busBuilder)
		{
			var container = new ContainerBuilder()
				.RegisterMicroBus(busBuilder)
				.Build();

			return container.Resolve<ICancelableMicroBus>();
		}

		private ICancelableMicroMediator GetMediator(BusBuilder busBuilder)
		{
			var container = new ContainerBuilder()
				.RegisterMicroBus(busBuilder)
				.Build();

			return container.Resolve<ICancelableMicroMediator>();
		}

		[Fact]
		public async Task DontCancel()
		{
			var mediator = GetMediator(new BusBuilder()
				.RegisterCancelableGlobalHandler<DelegatingHandler>()
				.RegisterCancelableHandler<CancelableMessage, CancelStage, MessageHandler>());

			var cancellationSource = new CancellationTokenSource();

			var result = await mediator.QueryAsync<CancelStage>(new CancelableMessage(3, cancellationSource), cancellationSource.Token);
			result.Should().Be(CancelStage.NotCancelled);

		}

		[Fact]
		public async Task CancelDelegatingHandler()
		{
			var mediator = GetMediator(new BusBuilder()
				.RegisterCancelableGlobalHandler<DelegatingHandler>()
				.RegisterCancelableHandler<CancelableMessage, CancelStage, MessageHandler>());

			var cancellationSource = new CancellationTokenSource();

			var result = await mediator.QueryAsync<CancelStage>(new CancelableMessage(1, cancellationSource), cancellationSource.Token);
			result.Should().Be(CancelStage.DelegatingHandler);
		}

		[Fact]
		public async Task CancelMessageHandler()
		{
			var mediator = GetMediator(new BusBuilder()
				.RegisterCancelableGlobalHandler<DelegatingHandler>()
				.RegisterCancelableHandler<CancelableMessage, CancelStage, MessageHandler>());

			var cancellationSource = new CancellationTokenSource();

			var result = await mediator.QueryAsync<CancelStage>(new CancelableMessage(2, cancellationSource), cancellationSource.Token);
			result.Should().Be(CancelStage.MessageHandler);
		}

		[Fact]
		public async Task CancelQueryHandler()
		{
			var bus = GetBus(new BusBuilder()
				.RegisterCancelableGlobalHandler<DelegatingHandler>()
				.RegisterCancelableHandler<CancelableMessage, CancelStage, MessageHandler>());

			var cancellationSource = new CancellationTokenSource();

			var result = await bus.QueryAsync(new CancelableQuery(2, cancellationSource), cancellationSource.Token);
			result.Should().Be(CancelStage.MessageHandler);
		}

		public class DelegatingHandler : ICancelableDelegatingHandler
		{
			public Task<object> Handle(INextHandler next, object message, CancellationToken cancellation)
			{
				(message as CancelableMessage)?.Tick();

				if (cancellation.IsCancellationRequested)
				{
					return Task.FromResult<object>(CancelStage.DelegatingHandler);
				}

				return next.Handle(message);
			}
		}

		public class MessageHandler : ICancelableMessageHandler<CancelableMessage, CancelStage>
		{
			public Task<CancelStage> Handle(CancelableMessage message, CancellationToken cancellation)
			{
				message?.Tick();

				if (cancellation.IsCancellationRequested) {
					return Task.FromResult(CancelStage.MessageHandler);
				}

				return Task.FromResult(CancelStage.NotCancelled);
			}
		}

		public class QueryHandler : ICancelableQueryHandler<CancelableQuery, CancelStage>
		{
			public Task<CancelStage> Handle(CancelableQuery query, CancellationToken cancellation)
			{
				query?.Tick();

				if (cancellation.IsCancellationRequested) {
					return Task.FromResult(CancelStage.MessageHandler);
				}

				return Task.FromResult(CancelStage.NotCancelled);
			}
		}

		public class CancelableQuery : CancelableMessage, IQuery<CancelableQuery, CancelStage>
		{
			public CancelableQuery(int ticksBeforeCancel, CancellationTokenSource cancellationTokenSource)
				: base(ticksBeforeCancel, cancellationTokenSource)
			{
			}
		}

		public class CancelableMessage
		{
			private readonly CancellationTokenSource cancellationTokenSource;

			private int ticksBeforeCancel;

			public CancelableMessage(int ticksBeforeCancel, CancellationTokenSource cancellationTokenSource)
			{
				this.ticksBeforeCancel = ticksBeforeCancel;
				this.cancellationTokenSource = cancellationTokenSource;
			}

			public void Tick()
			{
				ticksBeforeCancel -= 1;
				if (this.ticksBeforeCancel <= 0)
				{
					cancellationTokenSource.Cancel();
				}
			}
		}
	}
}
