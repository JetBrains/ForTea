using System;
using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Parsing;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Daemon.QuickFixes
{
	[QuickFix]
	public sealed class T4ReplaceWithFeatureBlockQuickFix : QuickFixBase
	{
		[NotNull]
		private StatementAfterFeatureError Highlighting { get; }

		public override string Text => "Replace with feature block";

		public T4ReplaceWithFeatureBlockQuickFix([NotNull] StatementAfterFeatureError highlighting) =>
			Highlighting = highlighting;

		protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
		{
			var node = Highlighting.BlockStart.GetParentOfType<IT4StatementBlock>().NotNull();
			string code = node.GetParentOfType<IT4CodeBlock>().NotNull().Code.GetText();
			var newNode = T4ElementFactory.CreateFeatureBlock(code);
			using (WriteLockCookie.Create(node.IsPhysical()))
			{
				ModificationUtil.ReplaceChild(node, newNode);
			}

			return null;
		}

		public override bool IsAvailable(IUserDataHolder cache) => Highlighting.IsValid();
	}
}
