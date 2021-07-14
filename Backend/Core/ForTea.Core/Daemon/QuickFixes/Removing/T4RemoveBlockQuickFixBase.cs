using System;
using GammaJul.ForTea.Core.Parsing;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Daemon.QuickFixes.Removing
{
	public abstract class T4RemoveBlockQuickFixBase<TNode, THighlighting> : QuickFixBase
		where TNode : IT4Block
		where THighlighting : IHighlighting
	{
		public override string Text => "Remove";

		[NotNull]
		protected THighlighting Highlighting { get; }

		[NotNull]
		protected abstract TNode Node { get; }

		protected T4RemoveBlockQuickFixBase([NotNull] THighlighting highlighting) => Highlighting = highlighting;

		protected sealed override Action<ITextControl> ExecutePsiTransaction(ISolution solution,
			IProgressIndicator progress)
		{
			using (WriteLockCookie.Create(Node.IsPhysical()))
			{
				var nextToken = Node.GetNextToken();
				if (nextToken != null && ShouldRemove(nextToken))
					ModificationUtil.DeleteChildRange(Node, nextToken);
				else ModificationUtil.DeleteChild(Node);
			}

			return null;
		}

		protected virtual bool ShouldRemove([NotNull] ITokenNode nextToken) =>
			nextToken.GetTokenType() == T4TokenNodeTypes.NEW_LINE;

		public sealed override bool IsAvailable(IUserDataHolder cache) => Highlighting.IsValid();
	}
}
