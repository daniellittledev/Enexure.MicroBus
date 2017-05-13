namespace Enexure.MicroBus.MicrosoftDependencyInjection
{
    public interface IMarker
    {
        bool ScopeCreated { get; set; }
    }
}