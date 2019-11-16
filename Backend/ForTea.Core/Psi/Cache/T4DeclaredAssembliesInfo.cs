using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Psi.Resolve.Macros;
using GammaJul.ForTea.Core.Psi.Resolve.Macros.Impl;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Cache
{
	public sealed class T4DeclaredAssembliesInfo
	{
		[NotNull, ItemNotNull]
		private readonly JetHashSet<IT4PathWithMacros> ReferencedAssemblies = new JetHashSet<IT4PathWithMacros>();

		private void HandleAssemblyDirective([NotNull] IT4AssemblyDirective directive)
		{
			string attributeName = T4DirectiveInfoManager.Assembly.NameAttribute.Name;
			string assemblyNameOrFile = directive.GetAttributeValueByName(attributeName);
			if (assemblyNameOrFile.IsNullOrWhitespace()) return;
			ReferencedAssemblies.Add(new T4PathWithMacros(assemblyNameOrFile, directive.GetSourceFile().NotNull()));
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

		public T4DeclaredAssembliesInfo([NotNull] IT4File baseFile, [NotNull] IT4FileDependencyGraph graph)
		{
			var projectFile = baseFile.GetSourceFile().NotNull().ToProjectFile().NotNull();
			var directives = graph
				.FindBestRoot(projectFile)
				.ToSourceFile()
				.NotNull()
				.GetPrimaryPsiFile()
				.NotNull()
				.Children()
				.OfType<IT4AssemblyDirective>();
			foreach (var directive in directives)
			{
				HandleAssemblyDirective(directive);
			}
		}
	}
}
