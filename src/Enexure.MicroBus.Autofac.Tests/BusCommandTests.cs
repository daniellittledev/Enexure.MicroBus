using System;
using System.Threading.Tasks;
using Autofac;
using Enexure.MicroBus.Annotations;
using FluentAssertions;
using Xunit;

namespace Enexure.MicroBus.Autofac.Tests
{
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

		[Fact]
		public async Task TestCommand()
		{
			var container = new ContainerBuilder().RegisterMicroBus(busBuilder => {

				return busBuilder
					.RegisterCommand<Command>().To<CommandHandler>();

			}).Build();

			var bus = container.Resolve<IMicroBus>();
			await bus.Send(new Command());
		}

		[Fact]
		public void TestMissingCommand()
		{
			var container = new ContainerBuilder().RegisterMicroBus(busBuilder => busBuilder).Build();

			var bus = container.Resolve<IMicroBus>();

			new Func<Task>(() => bus.Send(new Command()))
				.ShouldThrow<NoRegistrationForMessageException>();

		}
	}
}
