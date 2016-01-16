using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Enexure.MicroBus
{
	public sealed class Pipeline : IEnumerable<Type>
	{
		private static Pipeline pipeline;

		static Pipeline()
		{
			pipeline = new Pipeline();
		}

		readonly ImmutableList<Type> types = ImmutableList<Type>.Empty;

		public Pipeline()
		{
		}

		private Pipeline(ImmutableList<Type> types)
		{
			this.types = types;
		}

		public static Pipeline EmptyPipeline
		{
			get { return pipeline; }
		}

		public IEnumerator<Type> GetEnumerator()
		{
			return types.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return types.GetEnumerator();
		}

		public Pipeline AddHandler<T>()
			where T : IPipelineHandler
		{
			return new Pipeline(types.Add(typeof(T)));
		}

		public Pipeline AddHandlers(IEnumerable<Type> handlers)
		{
			var collection = handlers as IReadOnlyCollection<Type> ?? handlers.ToList();

			if (collection.Any(handler => !typeof(IPipelineHandler).GetTypeInfo().IsAssignableFrom(handler.GetTypeInfo()))) {
				throw new InvalidOperationException("Handlers must implement the IPipelineHandler interface");
			}

			return new Pipeline(types.AddRange(collection));
		}
	}
}
