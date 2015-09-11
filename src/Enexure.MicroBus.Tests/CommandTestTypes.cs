using System;
using System.Threading.Tasks;

namespace Enexure.MicroBus.Tests
{
	class CommandInterfaceHandler : ICommandHandler<ICommand>
	{
		public Task Handle(ICommand command)
		{
			throw new NotImplementedException();
		}
	}

	class CommandAHandler : ICommandHandler<CommandA>
	{
		public Task Handle(CommandA command)
		{
			throw new NotImplementedException();
		}
	}

	class OtherCommandAHandler : ICommandHandler<CommandA>
	{
		public Task Handle(CommandA command)
		{
			throw new NotImplementedException();
		}
	}

	class CommandBHandler : ICommandHandler<CommandB>
	{
		public Task Handle(CommandB command)
		{
			throw new NotImplementedException();
		}
	}

	class CommandCHandler : ICommandHandler<CommandC>
	{
		public Task Handle(CommandC @event)
		{
			throw new NotImplementedException();
		}
	}

	class CommandA : ICommand { }
	class CommandB : CommandA { }
	class CommandC : CommandB { }


	class CommandX : ICommand { }
	class CommandY : CommandX { }
	class CommandZ : CommandY { }
}
