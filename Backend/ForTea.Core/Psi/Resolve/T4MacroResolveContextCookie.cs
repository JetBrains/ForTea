using System;
using JetBrains.Annotations;
using JetBrains.ProjectModel;

namespace GammaJul.ForTea.Core.Psi.Resolve
{
	/// <summary>
	/// In T4, it's typical to include file from outside of solution.
	/// Such files are managed by <see cref="T4OutsideSolutionSourceFileManager"/> and lack project file.
	/// For such files we create context that contains
	/// the most suitable project file to be used for macro resolution
	/// </summary>
	public readonly struct T4MacroResolveContextCookie : IDisposable
	{
		[CanBeNull]
		public static IProjectFile ProjectFile { get; private set; }

		[NotNull]
		private static object Locker { get; } = new object();

		[CanBeNull]
		private IProjectFile Previous { get; }

		private T4MacroResolveContextCookie([CanBeNull] IProjectFile previous) => Previous = previous;
		public void Dispose() => ProjectFile = Previous;

		public static T4MacroResolveContextCookie Create(IProjectFile context)
		{
			lock (Locker)
			{
				var result = new T4MacroResolveContextCookie(ProjectFile);
				ProjectFile = context;
				return result;
			}
		}
	}
}
