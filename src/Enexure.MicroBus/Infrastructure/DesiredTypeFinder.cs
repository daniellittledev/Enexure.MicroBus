using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Enexure.MicroBus
{
	public class DesiredTypeFinder
	{
		private readonly Lazy<Type[]> allInstantiableTypes;

		public DesiredTypeFinder(params Assembly[] assemblies)
			: this(assemblies.ToList())
		{
		}

		public DesiredTypeFinder(IEnumerable<Assembly> assemblies)
		{
			allInstantiableTypes = new Lazy<Type[]>(() => PotentiallyInterestingTypes(assemblies));
		}

		private static Type[] PotentiallyInterestingTypes(IEnumerable<Assembly> assemblies)
		{
			return assemblies
				.SelectMany(a => a.GetTypes())
				.Where(t => t.IsInstantiable())
				.ToArray();
		}

	}
}
