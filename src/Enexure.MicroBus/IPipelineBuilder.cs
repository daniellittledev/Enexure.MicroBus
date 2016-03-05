using System;

namespace Enexure.MicroBus
{
	public interface IPipelineBuilder
	{
		Pipeline GetPipeline(Type messageType);
	}
}