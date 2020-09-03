using System.Linq;
using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Core;
using JetBrains.ForTea.RiderPlugin.TemplateProcessing.Services;
using JetBrains.ProjectModel;
using JetBrains.Rd.Tasks;
using JetBrains.ReSharper.Host.Features.ProjectModel.View;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.ReSharper.TestRunner.Abstractions.Extensions;
using JetBrains.Rider.Model;
using JetBrains.Util;

namespace JetBrains.ForTea.RiderPlugin.ProtocolAware.TestFramework
{
	[SolutionComponent]
	public sealed class T4TestHost
	{
		public T4TestHost(
			[NotNull] T4TestModel model,
			[NotNull] SolutionsManager solutionsManager
		) => model.PreprocessFile.Set(location =>
		{
			using var cookie = ReadLockCookie.Create();
			var solution = solutionsManager.Solution;
			if (solution == null) return Unit.Instance;

			var host = solution.GetComponent<ProjectModelViewHost>();
			var projectFile = host.GetItemById<IProjectFile>(location.Id).NotNull();
			var sourceFile = projectFile.ToSourceFile().NotNull();
			sourceFile.GetPsiServices().Files.CommitAllDocuments();
			var file = sourceFile.GetPsiFiles(T4Language.Instance).OfType<IT4File>().SingleItem().NotNull();

			var templatePreprocessingManager = solution.GetComponent<IT4TemplatePreprocessingManager>();
			templatePreprocessingManager.Preprocess(file);
			return Unit.Instance;
		});
	}
}
