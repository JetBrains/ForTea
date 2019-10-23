using System;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;

namespace GammaJul.ForTea.Core.Tree
{
	/// <summary>
	/// Serves booth as root of primary AST for T4 files
	/// and as an include in it
	/// </summary>
	public partial interface IT4File : IFileImpl
	{
		/// <summary>
		/// Primary PSI for a T4 file contains a lot of context,
		/// including the files that include the current T4 file
		/// and indirect includes.
		/// This is done in order to provide the most complete intelligent support.
		/// This means that PSI now doesn't just contain contents of the source file it corresponds to,
		/// but also a bunch of other files.
		/// <see cref="LogicalPsiSourceFile"/> is a reference to 
		/// </summary>
		[NotNull]
		IPsiSourceFile LogicalPsiSourceFile { get; }

		/// <summary>
		/// Which source file caused the current tree of T4 files to be built.
		/// Note that within the same tree there can be multiple nodes with different
		/// <see cref="LogicalPsiSourceFile"/> but all of them share the same <see cref="PhysicalPsiSourceFile"/>
		/// </summary>
		[CanBeNull]
		IPsiSourceFile PhysicalPsiSourceFile { get; }

		[Obsolete("You should state explicitly which source file you are interested in")]
		new IPsiSourceFile GetSourceFile();
	}
}
