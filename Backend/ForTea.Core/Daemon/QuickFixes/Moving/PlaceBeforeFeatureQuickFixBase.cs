using System;
using System.Linq;
using GammaJul.ForTea.Core.Parsing.Token;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Daemon.QuickFixes.Moving
{
	public abstract class PlaceBeforeFeatureQuickFixBase<TNode, THighlighting> : QuickFixBase
		where TNode : ITreeNode
		where THighlighting : IHighlighting
	{
		[NotNull]
		protected THighlighting Highlighting { get; }

		public override string Text => "Place before first feature block";
		protected PlaceBeforeFeatureQuickFixBase([NotNull] THighlighting highlighting) => Highlighting = highlighting;
		public override bool IsAvailable(IUserDataHolder cache) => Highlighting.IsValid();

		[NotNull]
		protected abstract TNode Node { get; }

		[NotNull]
		protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
		{
			var nodeToMove = Node;
			var file = nodeToMove.GetContainingFile() as IT4File;
			Assertion.AssertNotNull(file, "file != null");
			var feature = file.Blocks.OfType<IT4FeatureBlock>().First();

			ITreeNode featureBlock;
			using (WriteLockCookie.Create(file.IsPhysical()))
			{
				// clone the statement block and add it before the feature block
				var featurePrevSibling = feature.PrevSibling;
				featureBlock = ModificationUtil.CloneNode(nodeToMove, node => { });
				featureBlock = ModificationUtil.AddChildBefore(feature, featureBlock);

				// add a new line before the new statement block if needed
				if (featurePrevSibling != null && featurePrevSibling.GetTokenType() == T4TokenNodeTypes.NEW_LINE)
					ModificationUtil.AddChildAfter(featureBlock, T4TokenNodeTypes.NEW_LINE.CreateLeafElement());

				ModificationUtil.DeleteChild(nodeToMove);
			}

			return textControl =>
			{
				var range = featureBlock.GetDocumentRange().TextRange;
				textControl.Caret.MoveTo(range.EndOffset, CaretVisualPlacement.Generic);
			};
		}
	}
}
