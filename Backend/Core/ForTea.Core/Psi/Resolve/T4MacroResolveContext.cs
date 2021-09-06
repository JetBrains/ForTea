using System;
using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.Psi.OutsideSolution;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Resolve
{
	/// <summary>
	/// In T4, it's typical to include file from outside of solution.
	/// Such files are managed by <see cref="T4OutsideSolutionSourceFileManager"/> and lack project file.
	/// For such files we create context that contains
	/// the most suitable project file to be used for macro resolution
	/// </summary>
	public sealed class T4MacroResolveContext
	{
		[NotNull, ItemCanBeNull]
		private Stack<IProjectFile> ProjectFiles { get; } = new Stack<IProjectFile>();

		[CanBeNull]
		public IProjectFile MostSuitableProjectFile => ProjectFiles.Reverse().WhereNotNull().FirstOrDefault();

		[NotNull]
		public IDisposable RegisterNextLayer([CanBeNull] IProjectFile context)
		{
			ProjectFiles.Push(context);
			return new T4MacroResolveContextCookie(this);
		}

		private readonly struct T4MacroResolveContextCookie : IDisposable
		{
			[NotNull]
			private T4MacroResolveContext Provider { get; }

			public T4MacroResolveContextCookie([NotNull] T4MacroResolveContext provider) => Provider = provider;
			public void Dispose() => Provider.ProjectFiles.Pop();
		}
	}
}
