using System.Threading.Tasks;

namespace Enexure.MicroBus
{
	public interface IMicroBus
	{
		Task Send(ICommand busCommand);

		Task Publish(IEvent busEvent);

		Task<TResult> Query<TQuery, TResult>(IQuery<TQuery, TResult> query)
			where TQuery : IQuery<TQuery, TResult>
			where TResult : IResult;
	}
}
