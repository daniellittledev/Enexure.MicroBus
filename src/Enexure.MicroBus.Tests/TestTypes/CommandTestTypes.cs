using System;
using System.Threading.Tasks;

namespace Enexure.MicroBus.Tests
{
	class CommandInterfaceHandler : ICommandHandler<ICommand>
	{
		public Task Handle(ICommand Command)
		{
			throw new NotSupportedException();
		}
	}

	class CommandAHandler : ICommandHandler<CommandA>
	{
		public Task Handle(CommandA Command)
		{
			throw new NotSupportedException();
		}
	}

	class OtherCommandAHandler : ICommandHandler<CommandA>
	{
		public Task Handle(CommandA Command)
		{
			throw new NotSupportedException();
		}
	}

	class CommandBHandler : ICommandHandler<CommandB>
	{
		public Task Handle(CommandB Command)
		{
			throw new NotSupportedException();
		}
	}

	class CommandCHandler : ICommandHandler<CommandC>
	{
		public Task Handle(CommandC @event)
		{
			throw new NotSupportedException();
		}
	}

	class CommandA : ICommand { }
	class CommandB : CommandA { }
	class CommandC : CommandB { }


	class CommandX : ICommand { }
	class CommandY : CommandX { }
	class CommandZ : CommandY { }
}
