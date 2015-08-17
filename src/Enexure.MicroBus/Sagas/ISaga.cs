using System;

namespace Enexure.MicroBus.Sagas
{
	public interface ISaga<TData> : ISaga
		where TData : class
	{
		TData Data { get; set; }
	}

	public interface ISaga
	{
		Guid Id { get; set; }
	}
}