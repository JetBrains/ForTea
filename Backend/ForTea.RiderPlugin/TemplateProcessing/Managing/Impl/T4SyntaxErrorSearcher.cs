using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.Psi.Utils;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing.Impl
{
	[SolutionComponent]
	public class T4SyntaxErrorSearcher : IT4SyntaxErrorSearcher
	{
		public ITreeNode FindErrorElement(IT4File file)
		{
			var guard = new T4IncludeGuard<IPsiSourceFile>(EqualityComparer<IPsiSourceFile>.Default);
			return FindErrorElement(file, guard);
		}

		[CanBeNull]
		private static IErrorElement FindErrorElement(
			[NotNull] IT4File file,
			[NotNull] T4IncludeGuard<IPsiSourceFile> guard
		)
		{
			var sourceFile = file.GetSourceFile().NotNull();
			if (!guard.CanProcess(sourceFile)) return null;
			guard.StartProcessing(sourceFile);
			var error = file.ThisAndDescendants<IErrorElement>().ToEnumerable().FirstOrDefault();
			if (error != null) return error;
			var childError = file
				.Blocks
				.OfType<IT4IncludeDirective>()
				.SelectNotNull(include => include.Path.ResolveT4File(guard))
				.SelectNotNull(t4File => FindErrorElement(t4File, guard))
				.FirstOrDefault();
			guard.EndProcessing();
			return childError;
		}
	}
}
