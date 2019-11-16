using GammaJul.ForTea.Core.Daemon.Syntax;
using GammaJul.ForTea.Core.Psi;
using JetBrains.ReSharper.Daemon.SyntaxHighlighting;
using JetBrains.ReSharper.Host.Features.SyntaxHighlighting;
using JetBrains.ReSharper.Psi;

namespace JetBrains.ForTea.RiderPlugin.Daemon.Syntax
{
	[Language(typeof(T4Language))]
	public class T4RiderSyntaxHighlightingManager : RiderSyntaxHighlightingManager
	{
		public override SyntaxHighlightingProcessor CreateProcessor() => new T4SyntaxHighlightingProcessor();
	}
}
