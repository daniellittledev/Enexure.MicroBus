using System;

namespace Enexure.MicroBus
{
	public class GlobalPipelineProvider : IGlobalPipelineProvider
	{
		private Pipeline pipeline;

		public GlobalPipelineProvider(Pipeline pipeline)
		{
		    if (pipeline == null) throw new ArgumentNullException(nameof(pipeline));

		    this.pipeline = pipeline;
		}

	    public Pipeline GetGlobalPipeline()
		{
			return pipeline;
		}
	}
}