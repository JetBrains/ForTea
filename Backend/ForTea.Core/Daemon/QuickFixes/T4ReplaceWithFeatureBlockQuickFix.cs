using System;
using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Parsing;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Daemon.QuickFixes
{
	[QuickFix]
	public class T4ReplaceWithFeatureBlockQuickFix : QuickFixBase
	{
		[NotNull]
		private StatementAfterFeatureHighlighting Highlighting { get; }

		public override string Text => "Replace with feature block";

		public T4ReplaceWithFeatureBlockQuickFix([NotNull] StatementAfterFeatureHighlighting highlighting) =>
			Highlighting = highlighting;

		protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
		{
			var node = Highlighting.AssociatedNode;
			var newNode = T4ElementFactory.CreateFeatureBlock(node.GetCodeText());
			using (WriteLockCookie.Create(node.IsPhysical()))
			{
				ModificationUtil.ReplaceChild(node, newNode);
			}

			return null;
		}

		public override bool IsAvailable(IUserDataHolder cache) => Highlighting.IsValid();
	}
}
