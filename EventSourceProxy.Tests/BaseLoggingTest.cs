using NUnit.Framework;
using System.Diagnostics.Tracing;

namespace EventSourceProxy.Tests
{
	public class BaseLoggingTest
	{
		#region Setup and TearDown
		internal TestEventListener Listener;

		[SetUp]
		public void SetUp()
		{
			Listener = new TestEventListener();
		}
		#endregion

		protected void EnableLogging<TLog>() where TLog : class
		{
			// create the logger and make sure it is serializing the parameters properly
			var logger = EventSourceImplementer.GetEventSource<TLog>();
			Listener.EnableEvents(logger, EventLevel.LogAlways);
		}

		protected void EnableLogging(object proxy)
		{
			// create the logger and make sure it is serializing the parameters properly
			Listener.EnableEvents((EventSource)proxy, EventLevel.LogAlways);
		}
	}
}
