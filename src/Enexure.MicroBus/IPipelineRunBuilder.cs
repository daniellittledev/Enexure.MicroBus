using System;
using System.Threading.Tasks;

namespace Enexure.MicroBus
{
	public interface IPipelineRunBuilder
	{
		IInterceptorChain GetRunnerForPipeline(Type messageType);
	}
}