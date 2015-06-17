using System.Threading.Tasks;

namespace Enexure.MicroBus
{
	public interface ICommandHandler<in TCommand>
		where TCommand : ICommand
	{
		Task Handle(TCommand command);
	}
}
