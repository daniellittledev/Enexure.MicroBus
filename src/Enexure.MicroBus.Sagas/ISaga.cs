using System;

namespace Enexure.MicroBus.Sagas
{
    public interface ISaga
    {
        bool IsCompleted { get; }
    }
}