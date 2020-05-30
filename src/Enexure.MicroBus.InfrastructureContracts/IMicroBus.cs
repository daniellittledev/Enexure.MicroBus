using System.Threading.Tasks;

namespace Enexure.MicroBus
{
    using System.Threading;

    public interface IMicroBus
    {
        Task SendAsync(ICommand busCommand);

        Task PublishAsync(IEvent busEvent);

        Task<TResult> QueryAsync<TQuery, TResult>(IQuery<TQuery, TResult> query)
            where TQuery : IQuery<TQuery, TResult>;
    }

    public interface ICancelableMicroBus
    {
        Task SendAsync(ICommand busCommand, CancellationToken cancellation = default);

        Task PublishAsync(IEvent busEvent, CancellationToken cancellation = default);

        Task<TResult> QueryAsync<TQuery, TResult>(IQuery<TQuery, TResult> query, CancellationToken cancellation = default)
            where TQuery : IQuery<TQuery, TResult>;
    }
}
