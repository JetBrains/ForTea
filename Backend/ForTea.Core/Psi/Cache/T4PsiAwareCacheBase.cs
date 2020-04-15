using GammaJul.ForTea.Core.Psi.FileType;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.Lifetimes;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.Util.PersistentMap;

namespace GammaJul.ForTea.Core.Psi.Cache
{
	/// <typeparam name="TRequest">The type of part built on the background</typeparam>
	/// <typeparam name="TResponse">The result of handling the request on the main thread</typeparam>
	public abstract class T4PsiAwareCacheBase<TRequest, TResponse> : SimpleICache<TResponse> where TRequest : class
	{
		[NotNull]
		public override string Version => "12";

		protected T4PsiAwareCacheBase(
			Lifetime lifetime,
			IPersistentIndexManager persistentIndexManager,
			IUnsafeMarshaller<TResponse> valueMarshaller
		) : base(
			lifetime,
			persistentIndexManager,
			valueMarshaller
		)
		{
		}

		protected sealed override bool IsApplicable(IPsiSourceFile sf)
		{
			if (!base.IsApplicable(sf)) return false;
			// While it is technically possible to include
			// any file (a C++ file, for example)
			// into a T4 file and still get some valid code,
			// we are definitely not supporting that case
			// because wtf nobody does like that
			return sf.LanguageType is T4ProjectFileType;
		}

		[NotNull]
		public sealed override object Build(IPsiSourceFile sourceFile, bool isStartup)
		{
			// It is safe to access the PSI here.
			// According to SimpleICache documentation,
			// by the moment merge will be called,
			// PSI will have already been built
			var t4File = sourceFile.GetTheOnlyPsiFile<T4Language>() as IT4File;
			return Build(t4File.NotNull());
		}

		[NotNull]
		protected abstract TRequest Build([NotNull] IT4File file);
	}
}
