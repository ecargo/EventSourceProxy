using System;

namespace EventSourceProxy
{
	/// <summary>
	/// Specifies the TraceContextProvider to use for a class or interface.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false)]
	public sealed class TraceContextProviderAttribute : TraceProviderAttribute
	{
		/// <summary>
		/// Initializes a new instance of the TraceContextProviderAttribute class.
		/// </summary>
		/// <param name="providerType">The type of the provider to assign to this class or interface.</param>
		public TraceContextProviderAttribute(Type providerType) : base(providerType)
		{
		}
	}
}
