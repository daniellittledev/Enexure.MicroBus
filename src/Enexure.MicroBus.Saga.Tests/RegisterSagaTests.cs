using System;
using System.Threading.Tasks;
using Autofac;
using Enexure.MicroBus.Autofac;
using Enexure.MicroBus.Sagas;
using NUnit.Framework;

namespace Enexure.MicroBus.Saga.Tests
{
	[TestFixture]
	public class RegisterSagaTests
	{
		private class TestSaga : ISaga
		{
			public Guid Id { get; protected set;  }
			public bool IsCompleted { get; protected set; }
		}

		[Test]
		public void RegisterSaga()
		{
			var builder = new ContainerBuilder();
				
			var container = builder.RegisterMicroBus(busBuilder => busBuilder.RegisterSaga<TestSaga>()).Build();

			var bus = container.Resolve<IMicroBus>();
		}
	}
}
