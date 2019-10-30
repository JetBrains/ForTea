using System.Collections.Generic;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Interrupt;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Rider.Model;
using JetBrains.Util.dataStructures;
using Microsoft.CodeAnalysis;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing
{
	public interface IT4BuildMessageConverter
	{
		[NotNull]
		T4BuildResult ToT4BuildResult([NotNull, ItemNotNull] ICollection<Diagnostic> diagnostics, IT4File file);

		[NotNull]
		T4BuildResult ToT4BuildResult([NotNull] T4OutputGenerationException exception);

		[NotNull]
		List<T4BuildMessage> ToT4BuildMessages([NotNull] IEnumerable<T4FailureRawData> data);

		[NotNull]
		T4BuildResult SyntaxErrors([NotNull] IEnumerable<ITreeNode> nodes);

		[NotNull]
		T4BuildResult FatalError();
	}
}
