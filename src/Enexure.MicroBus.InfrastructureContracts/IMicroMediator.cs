using System.Threading.Tasks;

namespace Enexure.MicroBus
{
	public interface IMicroMediator
	{
		Task SendAsync(object message);
		Task PublishAsync(object message);
		Task<TResult> QueryAsync<TResult>(object message);
	}
}
