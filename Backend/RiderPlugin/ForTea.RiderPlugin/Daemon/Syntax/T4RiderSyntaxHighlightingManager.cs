using GammaJul.ForTea.Core.Psi;
using JetBrains.RdBackend.Common.Features.SyntaxHighlighting;
using JetBrains.ReSharper.Daemon.SyntaxHighlighting;
using JetBrains.ReSharper.Psi;

namespace JetBrains.ForTea.RiderPlugin.Daemon.Syntax
{
	[Language(typeof(T4Language))]
	public class T4RiderSyntaxHighlightingManager : RiderSyntaxHighlightingManager
	{
		public override SyntaxHighlightingProcessor CreateProcessor() => new T4SyntaxHighlightingProcessor();
	}
}
