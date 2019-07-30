using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;

namespace GammaJul.ForTea.Core.Psi.Resolve.Macros
{
	public sealed class T4IncludeRecursionGuard
	{
		[NotNull, ItemNotNull]
		private Stack<IPsiSourceFile> FilesBeingProcessed { get; }

		public T4IncludeRecursionGuard() => FilesBeingProcessed = new Stack<IPsiSourceFile>();
		public bool CanProcess([NotNull] IPsiSourceFile file) => !FilesBeingProcessed.Contains(file);
		public void StartProcessing([NotNull] IPsiSourceFile file) => FilesBeingProcessed.Push(file);
		public void EndProcessing() => FilesBeingProcessed.Pop();
		public bool IsOnTopLevel => FilesBeingProcessed.Count == 1;
	}
}
