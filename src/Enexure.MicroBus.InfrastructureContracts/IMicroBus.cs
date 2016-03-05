using System.Threading.Tasks;

namespace Enexure.MicroBus
{
	public interface IMicroBus
	{
		Task SendAsync(ICommand busCommand);

		Task PublishAsync(IEvent busEvent);

		Task<TResult> QueryAsync<TQuery, TResult>(IQuery<TQuery, TResult> query)
			where TQuery : IQuery<TQuery, TResult>;
	}
}
