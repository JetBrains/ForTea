using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.Psi.Resolve.Assemblies;
using GammaJul.ForTea.Core.Psi.Resolve.Macros.Impl;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Collections.Viewable;
using JetBrains.Diagnostics;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Model2.Assemblies.Interfaces;
using JetBrains.Rd;
using JetBrains.Rd.Impl;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.Rider.Model;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing.Impl
{
	public class T4TemplateInteracter
	{
		[NotNull]
		private IT4File File { get; }

		[NotNull]
		private IPsiSourceFile SourceFile => File.GetSourceFile().NotNull();

		[NotNull]
		private IProjectFile ProjectFile => SourceFile.ToProjectFile().NotNull();

		[NotNull]
		private IModuleReferenceResolveManager ResolveManager { get; }

		[NotNull]
		private T4AssemblyReferenceManager ReferenceManager { get; }

		[NotNull]
		private IT4AssemblyNamePreprocessor Preprocessor { get; }

		public T4TemplateInteracter([NotNull] ISolution solution, [NotNull] IT4File file)
		{
			File = file;
			ResolveManager = solution.GetComponent<IModuleReferenceResolveManager>();
			Preprocessor = solution.GetComponent<IT4AssemblyNamePreprocessor>();
			ReferenceManager = solution.GetComponent<T4AssemblyReferenceManager>();
			SetupModel();
		}

		private void SetupModel() =>
			SingleThreadScheduler.RunOnSeparateThread(Lifetime.Eternal, "Worker", scheduler =>
				scheduler.Queue(() =>
				{
					var client = new SocketWire.Server(Lifetime.Eternal, scheduler);
					var serializers = new Serializers();
					var Protocol = new Protocol("Server", serializers, new Identities(IdKind.Server), scheduler,
						client, Lifetime.Eternal);
					var model = new T4SubprocessProtocolModel(Lifetime.Eternal, Protocol);
					model.ResolveAssembly.Set(ResolveAssembly);
					model.ResolvePath.Set(rawPath => new T4PathWithMacros(rawPath, SourceFile).ResolvePath().FullPath);
				}));

		private string ResolveAssembly(string assembly)
		{
			var path = new T4PathWithMacros(assembly, SourceFile);
			string withMacrosExpanded = path.ResolveString();
			string preprocessed = Preprocessor.Preprocess(ProjectFile, withMacrosExpanded);
			var target = T4AssemblyReferenceManager.FindAssemblyReferenceTarget(preprocessed);
			var resolveContext = File.GetPsiModule().GetResolveContextEx(ProjectFile);
			var fileSystemPath = ResolveManager.Resolve(target, ProjectFile.GetProject(), resolveContext);
			return fileSystemPath?.FullPath ?? assembly;
		}
	}
}
