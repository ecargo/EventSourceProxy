using System;
using System.Diagnostics.CodeAnalysis;

namespace EventSourceProxy
{
	/// <summary>
	/// Specifies a TraceProvider for a class or interface.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
	[SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAtttributes", Justification = "Other attributes derive from this class")]
	public class TraceProviderAttribute : Attribute
	{
		/// <summary>
		/// Initializes a new instance of the TraceProviderAttribute class.
		/// </summary>
		/// <param name="providerType">The type of the provider to assign to this class or interface.</param>
		public TraceProviderAttribute(Type providerType)
		{
			ProviderType = providerType;
		}

		/// <summary>
		/// Gets the type of the provider to assign to the class or interface.
		/// </summary>
		public Type ProviderType { get; }
	}
}
