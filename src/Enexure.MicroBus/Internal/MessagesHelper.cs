using System;
using System.Collections.Generic;
using System.Linq;

namespace Enexure.MicroBus
{
	public static class MessagesHelper
	{
		public static IEnumerable<Type> ExpandType(Type type)
		{
			var isEvent = typeof(IEvent).IsAssignableFrom(type);
			var isCommand = typeof(ICommand).IsAssignableFrom(type);
			var isQuery = typeof(IQuery).IsAssignableFrom(type);

			return from subType in type.GetInterfaces().Concat(GetAllBaseTypes(type))
			       where isEvent && typeof(IEvent).IsAssignableFrom(subType)
			          || isCommand && typeof(ICommand).IsAssignableFrom(subType)
			          || isQuery && typeof(IQuery).IsAssignableFrom(subType)
			       select subType;
		}

		private static IEnumerable<Type> GetAllBaseTypes(Type type)
		{
			if (type.BaseType != null)
			{
				foreach (var baseType in GetAllBaseTypes(type.BaseType))
				{
					yield return baseType;
				}
			}

			if (type != typeof(object)) {
				yield return type;
			}
		}
	}
}
