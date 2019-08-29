using GammaJul.ForTea.Core.Psi;
using JetBrains.ReSharper.Daemon.SyntaxHighlighting;
using JetBrains.ReSharper.Host.Features.SyntaxHighlighting;
using JetBrains.ReSharper.Psi;

namespace GammaJul.ForTea.Core.Daemon.Syntax
{
	[Language(typeof(T4Language))]
	public class T4SyntaxHighlightingManager : RiderSyntaxHighlightingManager
	{
		public override SyntaxHighlightingProcessor CreateProcessor() => new T4SyntaxHighlightingProcessor();
	}
}
