using JetBrains.Annotations;
using JetBrains.Util;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing.Impl
{
	public sealed class T4ExecutionResultInFile : IT4ExecutionResult
	{
		[NotNull]
		private FileSystemPath OutputFile { get; }

		public T4ExecutionResultInFile([NotNull] FileSystemPath outputFile) => OutputFile = outputFile;
		public void Save(FileSystemPath destination) => OutputFile.MoveFile(destination, true);
	}
}
