using System;
using System.Threading.Tasks;

namespace Enexure.MicroBus
{
    [Obsolete(message: "Use IDelegatingHandler instead")]
    public interface IPipelineHandler
    {
        Task<object> Handle(Func<IMessage, Task<object>> next, IMessage message);
    }
}
