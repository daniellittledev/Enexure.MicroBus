using System;
using System.Threading.Tasks;

namespace Enexure.MicroBus.Sagas
{
	public class Saga<TData> : ISaga<TData>
		where TData : class
	{
		public Guid Id { get; set; }
		public TData Data { get; set; }
	}
}
