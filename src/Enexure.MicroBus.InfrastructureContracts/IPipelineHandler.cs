using System.Threading.Tasks;

namespace Enexure.MicroBus
{
	public interface IPipelineHandler
	{
		Task<object> Handle(IMessage message);
	}
}
