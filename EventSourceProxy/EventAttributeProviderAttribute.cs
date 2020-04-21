using System;

namespace EventSourceProxy
{
	/// <summary>
	/// Specifies the TraceSerializationProvider to use for a class or interface.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
	public sealed class EventAttributeProviderAttribute : TraceProviderAttribute
	{
		/// <summary>
		/// Initializes a new instance of the EventAttributeProviderAttribute class.
		/// </summary>
		/// <param name="providerType">The type of the provider to assign to this class or interface.</param>
		public EventAttributeProviderAttribute(Type providerType)
			: base(providerType)
		{
		}
	}
}
