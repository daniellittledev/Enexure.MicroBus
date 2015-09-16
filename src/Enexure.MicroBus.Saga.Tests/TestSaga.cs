using System;
using Enexure.MicroBus.Sagas;

namespace Enexure.MicroBus.Saga.Tests
{
	public class TestSaga : ISaga
	{
		public Guid Id { get; protected set; }
		public bool IsCompleted { get; protected set; }
	}
}