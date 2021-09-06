using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.Util.Dotnet.TargetFrameworkIds;

namespace GammaJul.ForTea.Core.Psi.Modules
{
	public interface IT4FilePsiModule : IProjectPsiModule
	{
		[NotNull]
		IPsiSourceFile SourceFile { get; }

		[CanBeNull]
		TargetFrameworkId OriginalTargetFrameworkId { get; }
	}
}
