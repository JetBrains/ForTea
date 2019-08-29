using System;
using GammaJul.ForTea.Core.Daemon.Highlightings;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.LiveTemplates.LiveTemplates;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Daemon.QuickFixes
{
	[QuickFix]
	public class ChangeAttributeValueQuickFix : QuickFixBase
	{
		[NotNull]
		private InvalidAttributeValueError Highlighting { get; }

		[NotNull]
		protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
		{
			string text = Highlighting.Value.GetText();
			DocumentRange range = Highlighting.Value.GetDocumentRange();

			// create a hotspot around the directive value, with basic completion invoked
			return textControl =>
				Shell.Instance
					.GetComponent<LiveTemplatesManager>()
					.CreateHotspotSessionAtopExistingText(
						solution,
						DocumentOffset.InvalidOffset,
						textControl,
						LiveTemplatesManager.EscapeAction.LeaveTextAndCaret,
						HotspotHelper.CreateBasicCompletionHotspotInfo(text, range))
					.Execute();
		}

		public override string Text => "Change value";

		public override bool IsAvailable(IUserDataHolder cache) => Highlighting.IsValid();

		public ChangeAttributeValueQuickFix([NotNull] InvalidAttributeValueError highlighting) =>
			Highlighting = highlighting;
	}
}
