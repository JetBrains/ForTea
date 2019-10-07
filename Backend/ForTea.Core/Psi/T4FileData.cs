using System.Collections.Generic;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Psi.Resolve.Macros;
using GammaJul.ForTea.Core.Psi.Resolve.Macros.Impl;
using GammaJul.ForTea.Core.Psi.Utils;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi {
	internal sealed class T4FileData {
		[NotNull, ItemNotNull]
		private readonly JetHashSet<IT4PathWithMacros> ReferencedAssemblies = new JetHashSet<IT4PathWithMacros>();

		private void HandleDirectives([NotNull] IT4File file)
		{
			var assemblyDirectives = file.Blocks.OfType<IT4AssemblyDirective>();
			foreach (var directive in assemblyDirectives)
			{
				HandleAssemblyDirective(directive);
			}
		}

		/// <summary>Handles an assembly directive.</summary>
		/// <param name="directive">The directive containing a potential assembly reference.</param>
		private void HandleAssemblyDirective([NotNull] IT4AssemblyDirective directive) {
			string assemblyNameOrFile = directive
				.GetAttributeValueByName(T4DirectiveInfoManager.Assembly.NameAttribute.Name);
			if (assemblyNameOrFile.IsNullOrWhitespace()) return;
			ReferencedAssemblies.Add(new T4PathWithMacros(assemblyNameOrFile, directive.GetSourceFile().NotNull()));
		}

		/// <summary>Computes a difference between this data and another one.</summary>
		/// <param name="oldData">The old data.</param>
		/// <returns>
		/// An instance of <see cref="T4FileDataDiff"/> containing the difference between the two data,
		/// or <c>null</c> if there are no differences.
		/// </returns>
		[CanBeNull]
		public T4FileDataDiff DiffWith([CanBeNull] T4FileData oldData) {

			if (oldData == null) {
				if (ReferencedAssemblies.Count == 0) return null;
				return new T4FileDataDiff(ReferencedAssemblies, EmptyList<IT4PathWithMacros>.InstanceList);
			}

			oldData.ReferencedAssemblies.Compare(
				ReferencedAssemblies,
				out JetHashSet<IT4PathWithMacros> addedAssemblies,
				out JetHashSet<IT4PathWithMacros> removedAssemblies
			);
			
			if (addedAssemblies.Count == 0 && removedAssemblies.Count == 0) return null;
			return new T4FileDataDiff(addedAssemblies, removedAssemblies);
		}

		/// <summary>Initializes a new instance of the <see cref="T4FileData"/> class.</summary>
		/// <param name="t4File">The T4 file that will be scanned for data.</param>
		public T4FileData([NotNull] IT4File t4File)
		{
			HandleDirectives(t4File);
			var guard = new T4IncludeGuard<IPsiSourceFile>(EqualityComparer<IPsiSourceFile>.Default);
			foreach (var includedFile in t4File.GetIncludedFilesRecursive(guard))
			{
				HandleDirectives(includedFile);
			}
		}

	}

}
