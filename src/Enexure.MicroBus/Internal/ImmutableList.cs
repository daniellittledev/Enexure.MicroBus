using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Enexure.MicroBus
{
	public class ImmutableList<T> : IReadOnlyCollection<T>
	{
		private static readonly ImmutableList<T> empty = new EmptyList();

		private readonly int count;
		private readonly T head;
		private readonly ImmutableList<T> tail;

		public static ImmutableList<T> Empty
		{
			get { return empty; }
		}

		public T Head
		{
			get { return head; }
		}

		public ImmutableList<T> Tail
		{
			get { return tail; }
		}

		protected ImmutableList(T head, ImmutableList<T> tail)
		{
			this.head = head;
			this.tail = tail;
			count = tail == null ? 0 : tail.count + 1;
		}

		public ImmutableList<T> AddRange(IEnumerable<T> items)
		{
			if (items == null) {
				return this;
			}

			var collection = items is IReadOnlyCollection<T> ? (IReadOnlyCollection<T>)items : items.ToList();

			if (collection.Count == 0) {
				return this;
			} else if (collection.Count == 1) {
				return new ImmutableList<T>(collection.Single(), this);
			} else {
				return collection.Aggregate(this, (current, item) => new ImmutableList<T>(item, current));
			}
		}

		public ImmutableList<T> Add(T item)
		{
			return new ImmutableList<T>(item, this);
		}

		public IEnumerator<T> GetEnumerator()
		{
			if (this == empty) {
				yield break;
			}

			yield return head;
			foreach (var elem in tail) {
				yield return elem;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private sealed class EmptyList : ImmutableList<T>
		{
			public EmptyList()
				: base(default(T), null)
			{
			}
		}

		public int Count {
			get { return count; }
		}
	}
}
