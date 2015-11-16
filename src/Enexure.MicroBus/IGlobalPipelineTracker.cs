namespace Enexure.MicroBus
{
	public interface IGlobalPipelineTracker
	{
		bool HasRun { get; }

		void MarkAsRun();
	}
}