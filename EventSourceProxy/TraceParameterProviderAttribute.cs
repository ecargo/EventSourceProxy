using System;

namespace EventSourceProxy
{
	/// <summary>
	/// Specifies the TraceParameterProvider to use for a given interface.
	/// </summary>
	[AttributeUsage(AttributeTargets.Interface)]
	public sealed class TraceParameterProviderAttribute : TraceProviderAttribute
	{
		/// <summary>
		/// Initializes a new instance of the TraceParameterProviderAttribute class.
		/// </summary>
		/// <param name="providerType">The type of provider to use for the given interface.</param>
		public TraceParameterProviderAttribute(Type providerType)
			: base(providerType)
		{
		}
	}
}
