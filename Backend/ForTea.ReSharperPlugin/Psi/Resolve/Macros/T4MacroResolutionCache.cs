using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using GammaJul.ForTea.Core.Psi.Cache;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
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
		private ILogger Logger { get; }

		public T4MacroResolutionCache(
			Lifetime lifetime,
			IPersistentIndexManager persistentIndexManager,
			[NotNull] ILogger logger
		) : base(lifetime, persistentIndexManager, T4MacroResolutionDataMarshaller.Instance) => Logger = logger;

		protected override T4MacroResolutionRequest Build(IT4File file)
		{
			var macros = file
				.GetThisAndChildrenOfType<IT4Macro>()
				.Distinct(macro => macro.RawAttributeValue.GetText());
			return new T4MacroResolutionRequest(macros.ToList());
		}

		public override void Merge(IPsiSourceFile sourceFile, object builtPart)
		{
			var request = (T4MacroResolutionRequest) builtPart.NotNull();
			var projectFile = sourceFile.ToProjectFile().NotNull();
			var macroNames = request.MacrosToResolve.Select(macro => macro.RawAttributeValue.GetText());
			var data = new T4MacroResolutionData(ResolveHeavyMacros(macroNames, projectFile));
			base.Merge(sourceFile, data);
		}

		[NotNull]
		private IReadOnlyDictionary<string, string> ResolveHeavyMacros(
			[NotNull] IEnumerable<string> macros,
			[NotNull] IProjectFile file
		)
		{
			var result = new Dictionary<string, string>();
			IVsBuildMacroInfo vsBuildMacroInfo = null;
			foreach (string macro in macros)
			{
				if (vsBuildMacroInfo == null)
				{
					vsBuildMacroInfo = TryGetVsBuildMacroInfo(file);
					if (vsBuildMacroInfo == null)
					{
						Logger.Error("Couldn't get IVsBuildMacroInfo");
						break;
					}
				}

				bool succeeded =
					HResultHelpers.SUCCEEDED(vsBuildMacroInfo.GetBuildMacroValue(macro, out string value))
					&& !string.IsNullOrEmpty(value);
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
