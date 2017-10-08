using System.Threading.Tasks;

namespace Enexure.MicroBus
{
    using System.Threading;

    public interface ICommandHandler<in TCommand>
        where TCommand : ICommand
    {
        Task Handle(TCommand command);
    }

    public interface ICancelableCommandHandler<in TCommand>
        where TCommand : ICommand
    {
        Task Handle(TCommand command, CancellationToken cancellation);
    }
}
