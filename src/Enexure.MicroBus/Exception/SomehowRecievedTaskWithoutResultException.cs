using System;

namespace Enexure.MicroBus
{
	public class SomehowRecievedTaskWithoutResultException : Exception
	{
		public SomehowRecievedTaskWithoutResultException()
			: base(string.Format("Tasks returned by handlers should return Task<?> but Task was returned instead, this is impossible"))
		{
		}
	}
}
