using System;
using System.Threading.Tasks;
using Autofac;
using Enexure.MicroBus.Tests.Annotations;
using FluentAssertions;
using NUnit.Framework;

namespace Enexure.MicroBus.Autofac.Tests
{
	[TestFixture]
	public class AutofacCommandTests
	{
		class Command : ICommand { }

		[UsedImplicitly]
		class CommandHandler : ICommandHandler<Command>
		{
			public Task Handle(Command command)
			{
				return Task.FromResult(0);
			}
		}

		[Test]
		public async Task TestCommand()
		{
			var container = new ContainerBuilder().RegisterMicroBus(busBuilder => {

				return busBuilder
					.RegisterCommand<Command>().To<CommandHandler>();

			}).Build();

			var bus = container.Resolve<IMicroBus>();
			await bus.Send(new Command());
		}

		[Test]
		public void TestMissingCommand()
		{
			var container = new ContainerBuilder().RegisterMicroBus(busBuilder => busBuilder).Build();

			var bus = container.Resolve<IMicroBus>();

			new Func<Task>(() => bus.Send(new Command()))
				.ShouldThrow<NoRegistrationForMessageException>();

		}
	}
}
