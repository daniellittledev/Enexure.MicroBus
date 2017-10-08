using System;

namespace Enexure.MicroBus
{
    public class BusSettings
    {
        public Synchronization HandlerSynchronization { get; set; }
    }

    public enum Synchronization
    {
        Syncronous,
        Asyncronous
    }
}
