namespace Enexure.MicroBus.Messages
{
    public class NoMatchingRegistrationEvent : IEvent
    {
        private readonly object message;

        public NoMatchingRegistrationEvent(object message)
        {
            this.message = message;
        }

        public object Message
        {
            get { return message; }
        }
    }
}
