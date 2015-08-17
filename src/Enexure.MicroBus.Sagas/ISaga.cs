using System;

namespace Enexure.MicroBus.Sagas
{
	public interface ISaga<out TData> : ISaga
		where TData : class
	{
		TData Data { get; }
	}

	public interface ISaga
	{
		Guid Id { get; }
		bool IsCompleted { get; }
	}
}