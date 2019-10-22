using System.Collections.Generic;
using System.Linq;
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
		protected IT4File File { get; }

		/// <summary>
		/// The source file from which the <see cref="File"/> is built.
		/// <see cref="File"/>.GetSourceFile() returns the initial file 
		/// </summary>
		[NotNull]
		protected IPsiSourceFile SourceFile { get; }

		[NotNull, ItemNotNull]
		protected IEnumerable<IT4File> Includes => File
			.BlocksEnumerable
			.OfType<IT4IncludeDirective>()
			.Select(it => it.LastChild)
			.OfType<IT4File>();

		protected T4RangeTranslatorBase([NotNull] IT4File file, [NotNull] IPsiSourceFile sourceFile)
		{
			File = file;
			SourceFile = sourceFile;
		}
	}
}
