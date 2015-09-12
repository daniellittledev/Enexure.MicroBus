using System;
using System.Collections.Generic;

namespace Enexure.MicroBus
{
	public class MultipleDifferentPipelinesRegisteredException : Exception
	{
		public IReadOnlyCollection<Pipeline> Pipelines { get; }

		public MultipleDifferentPipelinesRegisteredException(Type messageType, IReadOnlyCollection<Pipeline> pipelines)
			: base($"The message of type '{messageType.FullName}' has {pipelines.Count} different pipelines registered to it, messages may only correlate to a single pipeline. Make sure all the handlers registered to this message use the same pipeline.")
		{
			this.Pipelines = pipelines;
		}

		public MultipleDifferentPipelinesRegisteredException(Type messageType)
			: base($"The message of type '{messageType.FullName}' has multiple different pipelines registered to it, messages may only correlate to a single pipeline. Make sure all the handlers registered to this message and it's subtypes use the same pipeline.")
		{
		}
	}
}