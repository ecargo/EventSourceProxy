using System;
using System.Diagnostics.Tracing;
using System.Linq;

namespace EventSourceProxy.NetCoreApp3_1.Example
{
	public interface IExampleLogSource
	{
		[Event(1, Message="Starting")]
		void Starting();
		void AnEvent(string data);
		[Event(2, Message = "Stopping")]
		void Stopping();
	}
	
	public class Foo
	{
		public virtual void Bar() {}
		public virtual int Bar2() { return 1; }
	}

	public class TestListener : EventListener
	{
		protected override void OnEventWritten(EventWrittenEventArgs eventData)
		{
			Console.Write("Activity: {0} ", EventActivityScope.CurrentActivityId);
			Console.WriteLine(eventData.Message, eventData.Payload.ToArray());
		}
	}

	class Program
	{
		static void Main(string[] args)
		{
			// create the log
			var log = EventSourceImplementer.GetEventSourceAs<IExampleLogSource>();
			EventSource es = (EventSource)log;
			Console.WriteLine("Provider GUID = {0}", es.Guid);

			// create a listener
			var listener = new TestListener();
			listener.EnableEvents(es, EventLevel.LogAlways);

			using (new EventActivityScope())
			{
				log.Starting();
				for (int i = 0; i < 10; i++)
				{
					using (new EventActivityScope())
					{
						log.AnEvent(String.Format("i = {0}", i));
					}
				}
				log.Stopping();
			}

			TracingProxy.Create<Foo>(new Foo()).Bar();
			TracingProxy.Create<Foo>(new Foo()).Bar2();
		}
	}
}
