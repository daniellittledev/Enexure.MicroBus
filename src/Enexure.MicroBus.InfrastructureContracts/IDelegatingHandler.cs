using System.Threading.Tasks;

namespace Enexure.MicroBus
{
    using System.Threading;

    public interface IDelegatingHandler
    {
        Task<object> Handle(INextHandler next, object message);
    }

    public interface ICancelableDelegatingHandler
    {
        Task<object> Handle(INextHandler next, object message, CancellationToken cancellation);
    }

    public interface INextHandler
    {
        Task<object> Handle(object message);
    }
}