using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.Lifetimes;
using JetBrains.Rider.Model;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing
{
	public interface IT4TemplateCompiler
	{
		bool CanCompile([NotNull] IT4File file);

		T4BuildResult Compile(
			Lifetime lifetime,
			[NotNull] IT4File file,
			[CanBeNull] IProgressIndicator progress = null);
	}
}
