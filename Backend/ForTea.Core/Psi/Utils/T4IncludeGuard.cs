using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;

namespace GammaJul.ForTea.Core.Psi.Utils
{
	public sealed class T4IncludeGuard
	{
		[NotNull, ItemCanBeNull]
		private ISet<IPsiSourceFile> SeenFiles { get; }

		[NotNull, ItemCanBeNull]
		private Stack<IPsiSourceFile> FilesBeingProcessed { get; }

		public T4IncludeGuard()
		{
			FilesBeingProcessed = new Stack<IPsiSourceFile>();
			SeenFiles = new HashSet<IPsiSourceFile>();
		}

		public bool CanProcess([NotNull] IPsiSourceFile file) => !FilesBeingProcessed.Contains(file);

		public void StartProcessing([CanBeNull] IPsiSourceFile file)
		{
			FilesBeingProcessed.Push(file);
			SeenFiles.Add(file);
		}

		public bool HasSeenFile([NotNull] IPsiSourceFile file) => SeenFiles.Contains(file);
		public void EndProcessing() => FilesBeingProcessed.Pop();
		public bool IsOnTopLevel => FilesBeingProcessed.Count == 1;

		public void TryEndProcessing([CanBeNull] IPsiSourceFile file)
		{
			if (file == null) return;
			if (FilesBeingProcessed.Peek() == file) FilesBeingProcessed.Pop();
		}
	}
}
