using System.Collections.Generic;
using GammaJul.ForTea.Core.Psi.Resolve;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;

namespace GammaJul.ForTea.Core.Psi.Utils.Impl
{
	public sealed class T4ContextTrackingIncludeGuard : IT4IncludeGuard<IPsiSourceFile>
	{
		[NotNull, ItemCanBeNull]
		private ISet<IPsiSourceFile> SeenFiles { get; }

		[NotNull, ItemCanBeNull]
		private Stack<IPsiSourceFile> FilesBeingProcessed { get; }

		[NotNull]
		private Stack<T4MacroResolveContextCookie> Contexts { get; }

		public T4ContextTrackingIncludeGuard()
		{
			FilesBeingProcessed = new Stack<IPsiSourceFile>();
			Contexts = new Stack<T4MacroResolveContextCookie>();
			SeenFiles = new HashSet<IPsiSourceFile>();
		}

		public bool CanProcess(IPsiSourceFile file) => !FilesBeingProcessed.Contains(file);

		public void StartProcessing(IPsiSourceFile file)
		{
			FilesBeingProcessed.Push(file);
			Contexts.Push(T4MacroResolveContextCookie.Create(file.ToProjectFile()));
			SeenFiles.Add(file);
		}

		public bool HasSeenFile(IPsiSourceFile file) => SeenFiles.Contains(file);
		public void EndProcessing()
		{
			FilesBeingProcessed.Pop();
			Contexts.Pop().Dispose();
		}

		public bool IsOnTopLevel => FilesBeingProcessed.Count == 1;

		public void TryEndProcessing(IPsiSourceFile file)
		{
			if (file == null) return;
			if (EqualityComparer<IPsiSourceFile>.Default.Equals(FilesBeingProcessed.Peek(), file)) EndProcessing();
		}
	}
}
