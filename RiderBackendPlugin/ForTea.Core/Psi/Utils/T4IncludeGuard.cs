using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Utils
{
	public sealed class T4IncludeGuard
	{
		[NotNull, ItemCanBeNull]
		private ISet<FileSystemPath> SeenFiles { get; }

		[NotNull, ItemCanBeNull]
		private Stack<FileSystemPath> FilesBeingProcessed { get; }

		public T4IncludeGuard()
		{
			FilesBeingProcessed = new Stack<FileSystemPath>();
			SeenFiles = new HashSet<FileSystemPath>();
		}

		public bool CanProcess([NotNull] FileSystemPath file) => !FilesBeingProcessed.Contains(file);

		public void StartProcessing([NotNull] FileSystemPath file)
		{
			FilesBeingProcessed.Push(file);
			SeenFiles.Add(file);
		}

		public bool HasSeenFile([NotNull] FileSystemPath file) => SeenFiles.Contains(file);
		public void EndProcessing() => FilesBeingProcessed.Pop();

		public void TryEndProcessing([CanBeNull] FileSystemPath file)
		{
			if (file == null) return;
			if (EqualityComparer<FileSystemPath>.Default.Equals(FilesBeingProcessed.Peek(), file)) FilesBeingProcessed.Pop();
		}
	}
}
