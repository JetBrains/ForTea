using GammaJul.ForTea.Core.Psi;
using JetBrains.ReSharper.Daemon.Syntax;
using JetBrains.ReSharper.Daemon.SyntaxHighlighting;
using JetBrains.ReSharper.Psi;

namespace JetBrains.ForTea.RiderPlugin.Daemon.Syntax
{
	[Language(typeof(T4Language))]
	public class T4SyntaxHighlightingManager : SyntaxHighlightingManager
	{
		public override SyntaxHighlightingProcessor CreateProcessor() => new T4SyntaxHighlightingProcessor();
	}
}
