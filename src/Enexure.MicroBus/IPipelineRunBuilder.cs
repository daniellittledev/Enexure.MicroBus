using System;
using System.Threading.Tasks;

namespace Enexure.MicroBus
{
	public interface IPipelineRunBuilder
	{
		INextHandler GetRunnerForPipeline(Type messageType);
	}
}