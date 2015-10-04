using System;

namespace Enexure.MicroBus.Sagas
{
	public class SagaNotFoundException : Exception
	{
		public SagaNotFoundException()
			: base("Saga for predicate could not be found")
		{
		}
	}
}
