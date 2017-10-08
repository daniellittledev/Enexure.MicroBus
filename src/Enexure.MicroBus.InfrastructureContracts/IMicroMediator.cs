using System.Threading.Tasks;

namespace Enexure.MicroBus
{
    using System.Threading;

    public interface IMicroMediator
    {
        Task SendAsync(object message);
        Task PublishAsync(object message);
        Task<TResult> QueryAsync<TResult>(object message);
    }

    public interface ICancelableMicroMediator
    {
        Task SendAsync(object message, CancellationToken cancellation = default(CancellationToken));
        Task PublishAsync(object message, CancellationToken cancellation = default(CancellationToken));
        Task<TResult> QueryAsync<TResult>(object message, CancellationToken cancellation = default(CancellationToken));
    }
}
