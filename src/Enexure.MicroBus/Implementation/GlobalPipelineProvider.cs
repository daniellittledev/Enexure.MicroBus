using System;

namespace Enexure.MicroBus
{
	internal class GlobalPipelineProvider : IGlobalPipelineProvider
	{
		private Pipeline pipeline;

		public GlobalPipelineProvider(Pipeline pipeline)
		{
			this.pipeline = pipeline;
		}

		public Pipeline GetGlobalPipeline()
		{
			return pipeline;
		}
	}
}