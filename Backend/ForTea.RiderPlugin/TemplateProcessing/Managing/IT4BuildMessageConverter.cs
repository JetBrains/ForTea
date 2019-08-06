using System.Collections.Generic;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Rider.Model;
using Microsoft.CodeAnalysis;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing
{
	public interface IT4BuildMessageConverter
	{
		T4BuildResult ToT4BuildResult([NotNull, ItemNotNull] ICollection<Diagnostic> diagnostics, IT4File file);

		T4BuildResult FailedResult([NotNull] IT4File file);

		T4BuildResult FailedResult();
	}
}
