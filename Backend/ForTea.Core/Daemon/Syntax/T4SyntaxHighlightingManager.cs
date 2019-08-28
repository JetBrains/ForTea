using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.Psi.Resolve.Macros;
using JetBrains.Annotations;
using JetBrains.ReSharper.Daemon.SyntaxHighlighting;
using JetBrains.ReSharper.Host.Features.SyntaxHighlighting;
using JetBrains.ReSharper.Psi;

namespace GammaJul.ForTea.Core.Daemon.Syntax
{
	[Language(typeof(T4Language))]
	public class T4SyntaxHighlightingManager : RiderSyntaxHighlightingManager
	{
		[NotNull]
		private IT4MacroResolver Resolver { get; }

		public T4SyntaxHighlightingManager([NotNull] IT4MacroResolver resolver) => Resolver = resolver;
		public override SyntaxHighlightingProcessor CreateProcessor() => new T4SyntaxHighlightingProcessor(Resolver);
	}
}
