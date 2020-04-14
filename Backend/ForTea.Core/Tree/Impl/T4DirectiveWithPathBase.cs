using GammaJul.ForTea.Core.Psi.Resolve.Macros;
using GammaJul.ForTea.Core.Psi.Resolve.Macros.Impl;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;

namespace GammaJul.ForTea.Core.Tree.Impl
{
	public abstract class T4DirectiveWithPathBase : T4CompositeElement, IT4DirectiveWithPath
	{
		public IProjectFile ResolutionContext { get; set; }

		[CanBeNull]
		protected abstract string RawPath { get; }

		// Cannot use lazy here
		[CanBeNull]
		private IT4PathWithMacros CachedPath { get; set; }

		public IT4PathWithMacros GetOrCreatePath(IPsiSourceFile file = null) =>
			CachedPath ??= CreatePath(file ?? this.GetParentOfType<IT4FileLikeNode>().NotNull().LogicalPsiSourceFile);

		[NotNull]
		private IT4PathWithMacros CreatePath([NotNull] IPsiSourceFile logicalSourceFile) => new T4PathWithMacros(
			RawPath,
			logicalSourceFile,
			ResolutionContext
		);
	}
}
