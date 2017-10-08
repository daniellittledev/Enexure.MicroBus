using System;
using System.Threading.Tasks;

namespace Enexure.MicroBus
{
    using System.Threading;

    public interface IPipelineRunBuilder
    {
        INextHandler GetRunnerForPipeline(Type messageType, CancellationToken cancellation);
    }
}