using System;

namespace Enexure.MicroBus.Sagas
{
	public class Saga<TData> : ISaga<TData>
		where TData : class
	{
		public Guid Id { get; protected set; }
		public bool IsCompleted { get; protected set; }
		public TData Data { get; protected set; }
	}
}