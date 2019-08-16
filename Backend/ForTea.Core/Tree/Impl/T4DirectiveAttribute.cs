using GammaJul.ForTea.Core.Parsing;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;

namespace GammaJul.ForTea.Core.Tree.Impl {

	/// <summary>Represents a directive attribute, like <c>namespace="System"</c> in an import directive.</summary>
	public sealed class T4DirectiveAttribute : T4CompositeElement, IT4DirectiveAttribute {

		/// <summary>Gets the role of a child node.</summary>
		/// <param name="nodeType">The type of the child node</param>
		protected override T4TokenRole GetChildRole(NodeType nodeType) {
			if (nodeType == T4TokenNodeTypes.TOKEN)
				return T4TokenRole.Name;
			if (nodeType == T4TokenNodeTypes.EQUAL)
				return T4TokenRole.Separator;
			if (nodeType == T4ElementTypes.T4AttributeValue)
				return T4TokenRole.Value;
			return T4TokenRole.Unknown;
		}

		/// <summary>Gets the node type of this element.</summary>
		public override NodeType NodeType
			=> T4ElementTypes.T4DirectiveAttribute;

		/// <summary>Gets the token representing the name of this node.</summary>
		/// <returns>The name token, or <c>null</c> if none is available.</returns>
		public IT4Token GetNameToken()
			=> (IT4Token) FindChildByRole((short) T4TokenRole.Name);

		/// <summary>Gets the token representing the value of this attribute.</summary>
		/// <returns>A value token, or <c>null</c> if none is available.</returns>
		public IT4AttributeValue GetValueToken()
			=> FindChildByRole((short) T4TokenRole.Value) as IT4AttributeValue;

		/// <summary>Gets the name of the node.</summary>
		/// <returns>The node name, or <c>null</c> if none is available.</returns>
		public string GetName()
			=> GetNameToken()?.GetText();

		/// <summary>Gets the value of this attribute.</summary>
		/// <returns>The attribute value, or <c>null</c> if none is available.</returns>
		public string GetValue()
			=> GetValueToken()?.GetText();
	}

}
