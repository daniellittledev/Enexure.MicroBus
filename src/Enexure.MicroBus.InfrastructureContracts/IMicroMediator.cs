using System.Collections.Generic;
using System.Threading.Tasks;

namespace Enexure.MicroBus
{
	public interface IMicroMediator
	{
		Task SendAsync(object message);
		Task<IReadOnlyCollection<TResult>> QueryAllAsync<TResult>(object message);
		Task<TResult> QueryAsync<TResult>(object message);
	}
}
