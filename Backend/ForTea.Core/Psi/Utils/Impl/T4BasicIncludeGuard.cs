using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Utils.Impl
{
	public sealed class T4BasicIncludeGuard : IT4IncludeGuard<FileSystemPath>
	{
		[NotNull, ItemCanBeNull]
		private ISet<FileSystemPath> SeenFiles { get; }

		[NotNull, ItemCanBeNull]
		private Stack<FileSystemPath> FilesBeingProcessed { get; }

		public T4BasicIncludeGuard()
		{
			FilesBeingProcessed = new Stack<FileSystemPath>();
			SeenFiles = new HashSet<FileSystemPath>();
		}

		public bool CanProcess(FileSystemPath file) => !FilesBeingProcessed.Contains(file);

		public void StartProcessing(FileSystemPath file)
		{
			FilesBeingProcessed.Push(file);
			SeenFiles.Add(file);
		}

		public bool HasSeenFile(FileSystemPath file) => SeenFiles.Contains(file);
		public void EndProcessing() => FilesBeingProcessed.Pop();
		public bool IsOnTopLevel => FilesBeingProcessed.Count == 1;

		public void TryEndProcessing(FileSystemPath file)
		{
			if (file == null) return;
			if (EqualityComparer<FileSystemPath>.Default.Equals(FilesBeingProcessed.Peek(), file)) FilesBeingProcessed.Pop();
		}
	}
}
