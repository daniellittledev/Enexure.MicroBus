namespace Enexure.MicroBus.BuiltInEvents
{
	public class NoMatchingRegistrationEvent : IEvent
	{
		private readonly IMessage message;

		public NoMatchingRegistrationEvent(IMessage message)
		{
			this.message = message;
		}

		public IMessage Message
		{
			get { return message; }
		}
	}
}
