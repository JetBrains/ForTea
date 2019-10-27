using System.Collections.Generic;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;

namespace GammaJul.ForTea.Core.Parsing.Ranges
{
	public abstract class T4RangeTranslatorBase
	{
		/// <summary>
		/// The PSI file this translator is responsible for.
		/// It can be booth real T4 file and T4 file included into another one.
		/// </summary>
		[NotNull]
		protected IT4FileLikeNode IncludeOwner { get; }

		/// <summary>
		/// The source file from which the <see cref="IncludeOwner"/> is built.
		/// </summary>
		[NotNull]
		protected IPsiSourceFile SourceFile => IncludeOwner.LogicalPsiSourceFile;

		[NotNull, ItemNotNull]
		protected IEnumerable<IT4FileLikeNode> Includes => IncludeOwner.Includes;

		protected T4RangeTranslatorBase([NotNull] IT4FileLikeNode includeOwner) => IncludeOwner = includeOwner;
	}
}
