using GammaJul.ForTea.Core.Psi.FileType;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.UI.Icons;

namespace GammaJul.ForTea.Core.Psi.Service {

	/// <summary>Language service for T4 project files.</summary>
	[ProjectFileType(typeof(T4ProjectFileType))]
	public sealed class T4ProjectFileLanguageService : MixedProjectFileLanguageService {

		[NotNull] private readonly IT4Environment _t4Environment;

		/// <summary>Gets the PSI language type, <see cref="T4Language" />.</summary>
		protected override PsiLanguageType PsiLanguageType
			=> T4Language.Instance;

		/// <summary>
		/// Gets the T4 file icon.
		/// </summary>
		public override IconId Icon => T4.Id;

		public override IPsiSourceFileProperties GetPsiProperties(IProjectFile projectFile, IPsiSourceFile sourceFile, IsCompileService isCompileService)
			=> new T4PsiProjectFileProperties(projectFile, sourceFile, _t4Environment.IsSupported);

		/// <summary>Initializes a new instance of the <see cref="T4ProjectFileLanguageService" /> class.</summary>
		/// <param name="t4ProjectFileType">Type of the T4 project file.</param>
		/// <param name="t4Environment">The host environment.</param>
		public T4ProjectFileLanguageService([NotNull] T4ProjectFileType t4ProjectFileType, [NotNull] IT4Environment t4Environment)
			: base(t4ProjectFileType) => _t4Environment = t4Environment;
	}

}
