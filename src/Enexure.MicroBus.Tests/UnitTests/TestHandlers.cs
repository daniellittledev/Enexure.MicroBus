using System;
using System.Threading.Tasks;

namespace Enexure.MicroBus.Tests.UnitTests
{
	class CommandHandler<T> : ICommandHandler<T>
			where T : ICommand
	{
		public Task Handle(T command)
		{
			throw new NotSupportedException();
		}
	}

	class EventHandler<T> : IEventHandler<T>
			where T : IEvent
	{
		public Task Handle(T evnt)
		{
			throw new NotSupportedException();
		}
	}

	class QueryHandler<T, R> : IQueryHandler<T, R>
			where T : IQuery<T, R>
	{
		public Task<R> Handle(T query)
		{
			throw new NotSupportedException();
		}
	}

	class MessageHandler<T> : IMessageHandler<T, Unit>
	{
		public Task<Unit> Handle(T query)
		{
			throw new NotSupportedException();
		}
	}
}
