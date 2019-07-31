using JetBrains.ReSharper.Psi;

namespace GammaJul.ForTea.Core.Psi.Modules
{
	public interface IT4FilePsiModule
	{
		/// <summary>Returns the source file associated with this PSI module.</summary>
		IPsiSourceFile SourceFile { get; }
	}
}
