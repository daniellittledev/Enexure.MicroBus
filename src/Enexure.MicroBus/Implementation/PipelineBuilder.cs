using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Enexure.MicroBus
{
	public class PipelineBuilder : IPipelineBuilder
	{
		private readonly IReadOnlyCollection<Type> interceptorTypes;
		private readonly IDictionary<Type, IReadOnlyCollection<Type>> handlerLookup;

		public PipelineBuilder(BusBuilder busBuilder)
		{
			interceptorTypes = busBuilder.Interceptors.Select(x => x.InterceptorType).ToArray();
			handlerLookup = busBuilder.Registrations
				.ToLookup(x => x.MessageType, x => x.HandlerType)
				.Select(x => new { Key = x.Key, Handlers = x.Distinct() })
				.ToDictionary(x => x.Key, x => (IReadOnlyCollection<Type>)x.Handlers.ToArray());
		}

		private IEnumerable<HandlerRegistration> ExpandInheritedHandlers(HandlerRegistration registration)
		{
			return ReflectionExtensions
				.ExpandType(registration.MessageType)
				.Select(x => new HandlerRegistration(x, registration.HandlerType));
		}

		public Pipeline GetPipeline(Type messageType)
		{
			var raisedMessageTypes = ReflectionExtensions.ExpandType(messageType);

			var handlers = new List<Type>();
			foreach (var raisedMessageType in raisedMessageTypes)
			{
				if (handlerLookup.ContainsKey(raisedMessageType))
				{
					handlers.AddRange(handlerLookup[raisedMessageType]);
				}
			}
			return new Pipeline(interceptorTypes, handlers);
		}

		public void Validate()
		{
			var allCommands = handlerLookup
				.Where(x => typeof(ICommand).GetTypeInfo().IsAssignableFrom(x.Key.GetTypeInfo()))
				.Select(x => new { MessageType = x.Key, Count = x.Value.Count() })
				.Where(x => x.Count >= 2)
				.Select(x => x.MessageType);

			var allQueriers = handlerLookup
				.Where(x => typeof(IQuery).GetTypeInfo().IsAssignableFrom(x.Key.GetTypeInfo()))
				.Select(x => new { MessageType = x.Key, Count = x.Value.Count() })
				.Where(x => x.Count >= 2)
				.Select(x => x.MessageType);

			var invalidDuplicateRegistrations = allCommands.Concat(allQueriers).ToList();

			if (invalidDuplicateRegistrations.Any())
			{
				throw new InvalidDuplicateRegistrationsException(invalidDuplicateRegistrations);
			}
		}
	}
}
