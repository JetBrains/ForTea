using System;
using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Parsing;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Daemon.QuickFixes
{
	[QuickFix]
	public class T4RemoveDuplicateDirectiveQuickFix : QuickFixBase
	{
		[NotNull]
		private T4DuplicateDirectiveHighlighting Highlighting { get; }

		public T4RemoveDuplicateDirectiveQuickFix([NotNull] T4DuplicateDirectiveHighlighting highlighting) =>
			Highlighting = highlighting;

		public override bool IsAvailable(IUserDataHolder cache) => Highlighting.IsValid();
		public override string Text => "Remove duplicate directive";

		protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
		{
			var node = Highlighting.AssociatedNode;
			using (WriteLockCookie.Create(node.IsPhysical()))
			{
				var nextToken = node.GetNextToken();
				ITreeNode end;
				if (nextToken != null && nextToken.GetTokenType() == T4TokenNodeTypes.NEW_LINE) end = nextToken;
				else end = node;
				ModificationUtil.DeleteChildRange(node, end);
			}

			return null;
		}
	}
}
