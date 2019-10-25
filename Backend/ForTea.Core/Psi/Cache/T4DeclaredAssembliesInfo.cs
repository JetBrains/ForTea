using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Psi.Invalidation;
using GammaJul.ForTea.Core.Psi.Resolve.Macros;
using GammaJul.ForTea.Core.Psi.Resolve.Macros.Impl;
using GammaJul.ForTea.Core.Psi.Utils;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Cache
{
	internal sealed class T4DeclaredAssembliesInfo
	{
		[NotNull, ItemNotNull]
		private readonly JetHashSet<IT4PathWithMacros> ReferencedAssemblies = new JetHashSet<IT4PathWithMacros>();

		private void HandleAssemblyDirectives([NotNull] IT4File file)
		{
			var assemblyDirectives = file.Blocks.OfType<IT4AssemblyDirective>();
			string assemblyAttributeName = T4DirectiveInfoManager.Assembly.NameAttribute.Name;
			foreach (var directive in assemblyDirectives)
			{
				string assemblyNameOrFile = directive.GetAttributeValueByName(assemblyAttributeName);
				if (assemblyNameOrFile.IsNullOrWhitespace()) continue;
				ReferencedAssemblies.Add(new T4PathWithMacros(assemblyNameOrFile, directive.GetSourceFile().NotNull()));
			}
		}

		/// <summary>Computes a difference between this data and another one.</summary>
		/// <param name="oldDeclaredAssembliesInfo">The old data.</param>
		/// <returns>
		/// An instance of <see cref="T4DeclaredAssembliesDiff"/> containing the difference between the two data,
		/// or <c>null</c> if there are no differences.
		/// </returns>
		[CanBeNull]
		public T4DeclaredAssembliesDiff DiffWith([CanBeNull] T4DeclaredAssembliesInfo oldDeclaredAssembliesInfo)
		{
			if (oldDeclaredAssembliesInfo == null)
			{
				if (ReferencedAssemblies.Count == 0) return null;
				return new T4DeclaredAssembliesDiff(ReferencedAssemblies, EmptyList<IT4PathWithMacros>.InstanceList);
			}

			oldDeclaredAssembliesInfo.ReferencedAssemblies.Compare(
				ReferencedAssemblies,
				out JetHashSet<IT4PathWithMacros> addedAssemblies,
				out JetHashSet<IT4PathWithMacros> removedAssemblies
			);

			if (addedAssemblies.Count == 0 && removedAssemblies.Count == 0) return null;
			return new T4DeclaredAssembliesDiff(addedAssemblies, removedAssemblies);
		}

		public T4DeclaredAssembliesInfo([NotNull] IT4File baseFile, [NotNull] T4FileDependencyManager dependencyManager)
		{
			var rootLocation = dependencyManager.Graph.FindBestRoot(baseFile.GetSourceFile().NotNull().GetLocation());
			var rootFile = baseFile
				.GetSolution()
				.FindProjectItemsByLocation(rootLocation)
				.OfType<IProjectFile>()
				.Select(PsiSourceFileExtensions.ToSourceFile)
				.Single()
				.BuildT4Tree();
			foreach (var file in rootFile.GetThisIncludedFilesRecursive())
			{
				HandleAssemblyDirectives(file);
			}
		}
	}
}
