using System.Linq;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Utils
{
	public static class FileSystemPathExtensions
	{
		[NotNull]
		public static IProjectFile FindMostSuitableFile(
			[NotNull] this FileSystemPath path,
			[NotNull] IProjectFile requester
		)
		{
			var files = requester
				.GetSolution()
				.FindProjectItemsByLocation(path)
				.OfType<IProjectFile>()
				.AsList();
			Assertion.Assert(!files.IsEmpty(), "!files.IsEmpty()");
			if (files.IsSingle()) return files.Single();
			var requesterProject = requester.GetProject();
			var sameProjectFiles = files.Where(file => Equals(file.GetProject(), requesterProject)).AsList();
			if (sameProjectFiles.IsSingle()) return sameProjectFiles.Single();
			if (sameProjectFiles.IsEmpty()) return files.First();
			return sameProjectFiles.First();
		}
	}
}
