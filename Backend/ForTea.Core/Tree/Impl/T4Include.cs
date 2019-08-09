using System;
using GammaJul.ForTea.Core.Psi.Resolve.Macros;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;

namespace GammaJul.ForTea.Core.Tree.Impl {

	/// <summary>A T4 include. This is not a directive, it contains the included file tree.</summary>
	public sealed class T4Include : T4CompositeElement, IT4Include {

		public override NodeType NodeType => T4ElementTypes.T4Include;

		public bool Once { get; }
		public IT4PathWithMacros Path { get; }

		public T4Include() => throw new InvalidOperationException("Include is not supposed to be created this way");

		public T4Include(bool once, [NotNull] IT4PathWithMacros path)
		{
			Once = once;
			Path = path;
		}

		protected override T4TokenRole GetChildRole(NodeType nodeType)
			=> T4TokenRole.Unknown;
	}

}
