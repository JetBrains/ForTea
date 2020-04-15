using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Resolve.Macros.Impl
{
	public sealed class T4ResolvedPath
	{
		[NotNull]
		public string ResolvedPath { get; }

		[NotNull]
		public IPsiSourceFile SourceFile { get; }

		// Source file might have no corresponding project file;
		// This is the most suitable project file in that case
		public IProjectFile ProjectFile { get; }

		[CanBeNull]
		public FileSystemPath TryResolveAbsolutePath()
		{
			string expanded = ResolvedPath;

			// search as absolute path
			var asAbsolutePath = FileSystemPath.TryParse(expanded);
			if (asAbsolutePath.IsAbsolute) return asAbsolutePath;

			// search as relative path
			var asRelativePath = SourceFile.GetLocation().Directory.TryCombine(expanded);
			if (asRelativePath.IsAbsolute && asRelativePath.ExistsFile) return asRelativePath;

			return null;
		}

		public T4ResolvedPath(
			[NotNull] string resolvedPath,
			[NotNull] IPsiSourceFile sourceFile,
			[NotNull] IProjectFile projectFile
		)
		{
			ResolvedPath = resolvedPath;
			SourceFile = sourceFile;
			ProjectFile = projectFile;
		}
	}
}
