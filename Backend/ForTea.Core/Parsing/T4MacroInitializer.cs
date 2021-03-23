using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.Psi.Resolve;
using GammaJul.ForTea.Core.Psi.Resolve.Macros;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Parsing
{
	public sealed class T4MacroInitializer
	{
		[CanBeNull]
		private IPsiSourceFile LogicalSourceFile { get; }

		[NotNull]
		private T4MacroResolveContext Context { get; }

		[CanBeNull]
		private IT4MacroResolver MacroResolver { get; }

		public T4MacroInitializer(
			[CanBeNull] IPsiSourceFile logicalSourceFile,
			[NotNull] T4MacroResolveContext context,
			[CanBeNull] IT4MacroResolver macroResolver
		)
		{
			LogicalSourceFile = logicalSourceFile;
			Context = context;
			MacroResolver = macroResolver;
		}

		public bool CanResolveMacros =>
			LogicalSourceFile != null && Context.MostSuitableProjectFile != null && MacroResolver != null;

		public void ResolveMacros([NotNull] IT4File file)
		{
			var context = Context.MostSuitableProjectFile.NotNull();
			var logicalSourceFile = LogicalSourceFile.NotNull();
			var macros = new List<string>();
			foreach (var directive in file.BlocksEnumerable.OfType<IT4DirectiveWithPath>())
			{
				macros.AddRange(directive.RawMacros);
			}

			IReadOnlyDictionary<string, string> resolvedMacros;
			if (macros.IsEmpty()) resolvedMacros = EmptyDictionary<string, string>.Instance;
			else resolvedMacros = MacroResolver.NotNull().ResolveHeavyMacros(macros, context);
			foreach (var directive in file.BlocksEnumerable.OfType<IT4DirectiveWithPath>())
			{
				directive.InitializeResolvedPath(resolvedMacros, logicalSourceFile, context);
			}
		}
	}
}
