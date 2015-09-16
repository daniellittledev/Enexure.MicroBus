using Autofac;
using Enexure.MicroBus.Autofac;
using Enexure.MicroBus.Sagas;
using NUnit.Framework;

namespace Enexure.MicroBus.Saga.Tests
{
	[TestFixture]
	public class StartSagaTests
	{
		[Test]
		public void RegisterSaga()
		{
			var builder = new ContainerBuilder();

			var container = builder.RegisterMicroBus(busBuilder => busBuilder.RegisterSaga<TestSaga>()).Build();

			var bus = container.Resolve<IMicroBus>();
		}
	}
}
