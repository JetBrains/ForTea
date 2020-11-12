using GammaJul.ForTea.Core.Daemon.Syntax;
using GammaJul.ForTea.Core.Psi;
using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Daemon.SyntaxHighlighting;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;

namespace JetBrains.ForTea.ReSharperPlugin.Daemon.Syntax
{
	[Language(typeof(T4Language))]
	public class T4ReSharperSyntaxHighlightingManager : SyntaxHighlightingManager
	{
		[NotNull]
		public override SyntaxHighlightingStageProcess CreateProcess(
			IDaemonProcess process,
			IContextBoundSettingsStore settings,
			IFile getPrimaryPsiFile
		)
		{
			var processor = new T4SyntaxHighlightingProcessor();
			return new SyntaxHighlightingStageProcess(process, settings, getPrimaryPsiFile, processor);
		}
	}
}
