using System;

namespace Enexure.MicroBus
{
	public class BusSettings
	{
		public Concurrency HanlderConcurrency { get; set; }
	}

	public enum Concurrency
	{
		Syncronous,
		Asyncronous
	}
}
