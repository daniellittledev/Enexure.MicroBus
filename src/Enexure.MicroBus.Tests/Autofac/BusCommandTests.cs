using System;
using System.Threading.Tasks;
using Autofac;
using Enexure.MicroBus.Autofac;
using Enexure.MicroBus.Tests.Common;
using FluentAssertions;
using NUnit.Framework;

namespace Enexure.MicroBus.Tests.Autofac
{
	[TestFixture]
	public class AutofacCommandTests
	{
		class Command : ICommand { }

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

			var pipline = new Pipeline()
				.AddHandler<PipelineHandler>();

			var container = new ContainerBuilder().RegisterMicroBus(busBuilder => {

				return busBuilder
					.RegisterCommand<Command>().To<CommandHandler>(pipline);

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
