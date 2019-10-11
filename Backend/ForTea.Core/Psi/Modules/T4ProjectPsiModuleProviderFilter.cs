using System;
using GammaJul.ForTea.Core.Psi.Resolve.Macros;
using GammaJul.ForTea.Core.TemplateProcessing.Services;
using JetBrains.Annotations;
using JetBrains.Application.changes;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;

namespace GammaJul.ForTea.Core.Psi.Modules {

	/// <summary>Provides a <see cref="T4ProjectPsiModuleHandler"/> for a given project.</summary>
	[SolutionComponent]
	sealed class T4ProjectPsiModuleProviderFilter : IProjectPsiModuleProviderFilter {
		[NotNull] private readonly ChangeManager _changeManager;
		[NotNull] private readonly IT4Environment _t4Environment;
		[NotNull] private readonly PsiProjectFileTypeCoordinator _coordinator;
		
		[NotNull]
		private IT4TemplateKindProvider TemplateDataManager { get; }

		[NotNull]
		public Tuple<IProjectPsiModuleHandler, IPsiModuleDecorator> OverrideHandler(
			Lifetime lifetime,
			[NotNull] IProject project,
			[NotNull] IProjectPsiModuleHandler handler
		) {
			var t4ModuleHandler = new T4ProjectPsiModuleHandler(
				lifetime,
				handler,
				_changeManager,
				_t4Environment,
				project,
				_coordinator,
				TemplateDataManager
			);
			return new Tuple<IProjectPsiModuleHandler, IPsiModuleDecorator>(t4ModuleHandler, null);
		}

		public T4ProjectPsiModuleProviderFilter(
			[NotNull] ChangeManager changeManager,
			[NotNull] IT4Environment t4Environment,
			[NotNull] IT4MacroResolver resolver,
			[NotNull] PsiProjectFileTypeCoordinator coordinator,
			[NotNull] IT4TemplateKindProvider templateDataManager
		)
		{
			_changeManager = changeManager;
			_t4Environment = t4Environment;
			_coordinator = coordinator;
			TemplateDataManager = templateDataManager;
		}

	}

}
