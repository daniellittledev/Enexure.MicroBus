using System;
using System.Threading.Tasks;
using Enexure.MicroBus;
using Enexure.MicroBus.Tests.Common;
using FluentAssertions;
using NUnit.Framework;

namespace Enexure.MicroBus.Tests
{
	[TestFixture]
	public class CommandTests
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
		public void NoHandlerShouldThrow()
		{
			var bus = new BusBuilder()
				.BuildBus();

			var func = (Func<Task>)(() => bus.Send(new Command()));

			func.ShouldThrowExactly<NoRegistrationForMessage>();
		}

		[Test]
		public async Task TestCommand()
		{
			var pipline = new Pipeline()
				.AddHandler<PipelineHandler>();

			var bus = new BusBuilder()
				.RegisterCommand<Command>().To<CommandHandler>(pipline)
				.BuildBus();

			await bus.Send(new Command());

		}

		[Test]
		public void TestMissingCommand()
		{
			var bus = new BusBuilder()
				.BuildBus();

			new Func<Task>(() => bus.Send(new Command()))
				.ShouldThrow<NoRegistrationForMessage>();

		}
	}
}
