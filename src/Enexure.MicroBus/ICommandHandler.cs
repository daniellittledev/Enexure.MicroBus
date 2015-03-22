using System;
using System.Threading.Tasks;
using Enexure.MicroBus.MessageContracts;

namespace Enexure.MicroBus
{
	public interface ICommandHandler<in TCommand>
		where TCommand : ICommand
	{
		Task Handle(TCommand command);
	}
}
