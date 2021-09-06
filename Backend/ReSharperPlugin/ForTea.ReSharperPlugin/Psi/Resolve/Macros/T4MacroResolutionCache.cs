using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using GammaJul.ForTea.Core.Psi.Cache;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Threading;
using JetBrains.DataFlow;
using JetBrains.Diagnostics;
using JetBrains.Interop.WinApi;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.Util;
using JetBrains.VsIntegration.ProjectModel;
using Microsoft.VisualStudio.Shell.Interop;

namespace JetBrains.ForTea.ReSharperPlugin.Psi.Resolve.Macros
{
	/// <summary>
	/// In R#, it is only possible to resolve macros on the main thread,
	/// so we have to cache them to be able to access them from the daemon
	/// </summary>
	[PsiComponent]
	public sealed class T4MacroResolutionCache : T4PsiAwareCacheBase<T4MacroResolutionRequest, T4MacroResolutionData>
	{
		[NotNull]
		public Signal<IPsiSourceFile> OnFileMarkedForInvalidation { get; }

		public T4MacroResolutionCache(
			Lifetime lifetime,
			[NotNull] IShellLocks locks,
			[NotNull] IPersistentIndexManager persistentIndexManager,
			[NotNull] ISolution solution
		) : base(lifetime, locks, persistentIndexManager, T4MacroResolutionDataMarshaller.Instance) =>
			OnFileMarkedForInvalidation = new(lifetime, "Update from T4MacroResolutionCache");

		[NotNull]
		protected override T4MacroResolutionRequest Build(IT4File file)
		{
			var macros = file
				.GetThisAndChildrenOfType<IT4Macro>()
				.Where(macro => macro.RawAttributeValue != null)
				.Distinct(macro => macro.RawAttributeValue.GetText());
			return new T4MacroResolutionRequest(macros.ToList());
		}

		public override void Merge(IPsiSourceFile sourceFile, object builtPart)
		{
			var request = (T4MacroResolutionRequest) builtPart.NotNull();
			var projectFile = sourceFile.ToProjectFile().NotNull();
			var macroNames = request.MacrosToResolve.Select(macro => macro.RawAttributeValue.GetText());
			var oldKeys = Map.TryGetValue(sourceFile)?.ResolvedMacros.Keys ?? EmptyList<string>.Instance;
			var data = new T4MacroResolutionData(ResolveHeavyMacros(macroNames, projectFile));
			var newKeys = data.ResolvedMacros.Keys;
			base.Merge(sourceFile, data);

			if (!newKeys.All(it => oldKeys.Contains(it)))
			{
				OnFileMarkedForInvalidation.Fire(sourceFile);
			}
		}

		[NotNull]
		private IReadOnlyDictionary<string, string> ResolveHeavyMacros(
			[NotNull] IEnumerable<string> macros,
			[NotNull] IProjectFile file
		)
		{
			var result = new Dictionary<string, string>();
			Lazy<IVsBuildMacroInfo> vsBuildMacroInfo = Lazy.Of(() => TryGetVsBuildMacroInfo(file), false);
			foreach (string macro in macros)
			{
				bool succeeded = false;
				string value = null;
				if (vsBuildMacroInfo.Value != null)
				{
					succeeded = HResultHelpers.SUCCEEDED(vsBuildMacroInfo.Value.GetBuildMacroValue(macro, out value)) &&
					            !string.IsNullOrEmpty(value);
				}

				if (!succeeded)
				{
					value = MSBuildExtensions.GetStringValue(T4ResolutionUtils.TryGetVsHierarchy(file), macro, null);
					succeeded = !string.IsNullOrEmpty(value);
				}

				if (succeeded)
				{
					result[macro] = value;
				}
			}

			return result;
		}

		/// <summary>
		/// The <see cref="IVsHierarchy"/> representing the project file
		/// normally implements <see cref="IVsBuildMacroInfo"/>.
		/// </summary>
		/// <returns>An instance of <see cref="IVsBuildMacroInfo"/> if found.</returns>
		[CanBeNull, SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
		private static IVsBuildMacroInfo TryGetVsBuildMacroInfo([NotNull] IProjectFile file) =>
			T4ResolutionUtils.TryGetVsHierarchy(file) as IVsBuildMacroInfo;
	}
}
