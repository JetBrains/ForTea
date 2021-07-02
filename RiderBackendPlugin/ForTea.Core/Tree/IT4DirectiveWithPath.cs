using System.Collections.Generic;
using GammaJul.ForTea.Core.Psi.Resolve.Macros.Impl;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;

namespace GammaJul.ForTea.Core.Tree
{
	public interface IT4DirectiveWithPath : IT4TreeNode
	{
		[NotNull, ItemNotNull]
		IEnumerable<string> RawMacros { get; }

		[NotNull]
		T4ResolvedPath ResolvedPath { get; }

		void InitializeResolvedPath(
			[NotNull] IReadOnlyDictionary<string, string> resolveResults,
			[NotNull] IPsiSourceFile sourceFile,
			[NotNull] IProjectFile projectFile
		);
	}
}
