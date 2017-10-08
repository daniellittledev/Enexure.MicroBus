using System;

namespace Enexure.MicroBus
{
    public class OuterPipelineDetector : IOuterPipelineDetector, IOuterPipelineDetertorUpdater
    {
        int markers = 0;

        public bool IsOuterPipeline
        {
            get { return markers == 0; }
        }

        void IOuterPipelineDetertorUpdater.PopMarker()
        {
            markers -= 1;
        }

        void IOuterPipelineDetertorUpdater.PushMarker()
        {
            markers += 1;
        }
    }
}
