using System;

namespace EventSourceProxy
{
	/// <summary>
	/// Specifies that a given parameter should not be traced.
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
	public sealed class TraceIgnoreAttribute : Attribute
	{
		/// <summary>
		/// Initializes a new instance of the TraceIgnoreAttribute class.
		/// </summary>
		public TraceIgnoreAttribute()
		{
		}
	}
}
