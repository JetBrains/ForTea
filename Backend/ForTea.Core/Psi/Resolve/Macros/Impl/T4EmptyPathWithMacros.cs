using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Resolve.Macros.Impl
{
	public sealed class T4EmptyPathWithMacros : IT4PathWithMacros
	{
		public T4EmptyPathWithMacros([NotNull] IProjectFile projectFile) => ProjectFile = projectFile;
		public IPsiSourceFile Resolve() => null;
		public FileSystemPath ResolvePath() => FileSystemPath.Empty;
		public string ResolveString() => "";
		public IProjectFile ProjectFile { get; }
		public string RawPath => "";
	}
}
