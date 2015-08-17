using System;

namespace Enexure.MicroBus.Sagas
{
	public static class BusBuilderExtensions
	{
		public static IBusBuilder RegisterSaga<TSaga, TSagaData>(this IBusBuilder busBuilder, TSaga saga)
			where TSaga : ISaga<TSagaData>
			where TSagaData : class
		{

			return busBuilder;
		}
	}
}
