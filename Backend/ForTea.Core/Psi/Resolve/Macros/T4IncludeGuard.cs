using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.Util.dataStructures;

namespace GammaJul.ForTea.Core.Psi.Resolve.Macros
{
	public sealed class T4IncludeGuard
	{
		private FrugalLocalList<IPsiSourceFile> SeenFiles { get; }

		[NotNull, ItemNotNull]
		private Stack<IPsiSourceFile> FilesBeingProcessed { get; }

		public T4IncludeGuard() => FilesBeingProcessed = new Stack<IPsiSourceFile>();
		public bool CanProcess([NotNull] IPsiSourceFile file) => !FilesBeingProcessed.Contains(file);

		public void StartProcessing([NotNull] IPsiSourceFile file)
		{
			FilesBeingProcessed.Push(file);
			SeenFiles.Add(file);
		}

		public bool HasSeenFile([NotNull] IPsiSourceFile file) => SeenFiles.Contains(file);
		public void EndProcessing() => FilesBeingProcessed.Pop();
		public bool IsOnTopLevel => FilesBeingProcessed.Count == 1;
	}
}
