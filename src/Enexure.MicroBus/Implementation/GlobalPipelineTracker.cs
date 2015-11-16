using System;

namespace Enexure.MicroBus
{
	public class GlobalPipelineTracker : IGlobalPipelineTracker
	{
		public bool HasRun
		{
			get; private set;
		}

		public void MarkAsRun()
		{
			HasRun = true;
		}
	}
}
