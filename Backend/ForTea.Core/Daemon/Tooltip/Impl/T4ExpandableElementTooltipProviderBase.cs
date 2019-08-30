using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Feature.Services.Descriptions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl.DocumentMarkup;

namespace GammaJul.ForTea.Core.Daemon.Tooltip.Impl
{
	public abstract class T4ExpandableElementTooltipProviderBase<TExpandable> : IdentifierTooltipProvider<T4Language>
		where TExpandable : class, ITreeNode
	{
		protected T4ExpandableElementTooltipProviderBase(
			Lifetime lifetime,
			ISolution solution,
			IDeclaredElementDescriptionPresenter presenter
		) : base(lifetime, solution, presenter)
		{
		}

		[NotNull]
		public override string GetTooltip([NotNull] IHighlighter highlighter)
		{
			if (!highlighter.IsValid) return string.Empty;
			var psiServices = Solution.GetPsiServices();
			if (!psiServices.Files.AllDocumentsAreCommitted) return string.Empty;
			if (psiServices.Caches.HasDirtyFiles) return string.Empty;
			var document = highlighter.Document;
			var sourceFile = document.GetPsiSourceFile(Solution);
			if (sourceFile?.IsValid() != true) return string.Empty;
			var documentRange = new DocumentRange(document, highlighter.Range);
			if (!(GetPsiFile(sourceFile, documentRange) is IT4File psiFile)) return string.Empty;
			if (!(psiFile.FindTokenAt(documentRange.StartOffset) is IT4Token token)) return string.Empty;
			var expandable = token.GetParentOfType<TExpandable>();
			if (expandable == null) return string.Empty;
			string expanded = Expand(expandable);
			if (expanded == null) return string.Empty;
			return $"({ExpandableName}) {expanded}";
		}

		[CanBeNull]
		protected abstract string Expand([NotNull] TExpandable expandable);

		[NotNull]
		protected abstract string ExpandableName { get; }
	}
}
