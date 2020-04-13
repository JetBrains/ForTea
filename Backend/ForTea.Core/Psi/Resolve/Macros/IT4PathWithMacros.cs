using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Resolve.Macros
{
	public interface IT4PathWithMacros
	{
		/// <note>
		/// Implementation caches that value
		/// </note>
		[NotNull]
		string ResolveString();

		[CanBeNull]
		FileSystemPath TryResolveAbsolutePath();

		[NotNull]
		IProjectFile ProjectFile { get; }

		[NotNull]
		IPsiSourceFile SourceFile { get; }
	}
}
