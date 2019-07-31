using GammaJul.ForTea.Core.Psi.Resolve.Macros;
using JetBrains.Annotations;

namespace GammaJul.ForTea.Core.Tree
{
	/// <summary>epresents a T4 directive attribute, like namespace in import directive.</summary>
	public interface IT4DirectiveAttribute : IT4NamedNode
	{
		/// <summary>Gets the token representing the equal sign between the name and the value of this attribute.</summary>
		/// <returns>An equal token, or <c>null</c> if none is available.</returns>
		[CanBeNull]
		IT4Token GetEqualToken();

		/// <summary>Gets the token representing the value of this attribute.</summary>
		/// <returns>A value token, or <c>null</c> if none is available.</returns>
		[CanBeNull]
		IT4AttributeValue GetValueToken();

		/// <summary>Gets the value of this attribute.</summary>
		/// <returns>The attribute value, or <c>null</c> if none is available.</returns>
		[CanBeNull]
		string GetValue();

		[CanBeNull]
		IT4PathWithMacros Reference { get; }
	}
}
