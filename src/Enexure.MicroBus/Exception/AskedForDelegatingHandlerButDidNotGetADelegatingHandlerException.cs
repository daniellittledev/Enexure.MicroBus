namespace Enexure.MicroBus
{
    using System;

    public class AskedForDelegatingHandlerButDidNotGetADelegatingHandlerException : Exception
    {
        public override string Message => "Asked for delegating handler but did not get a delegating handler";
    }
}