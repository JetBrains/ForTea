using System.Linq;
using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.DataContext;
using JetBrains.Application.UI.Actions;
using JetBrains.Application.UI.ActionsRevised.Menu;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.DataContext;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.DataContext;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.Util;

namespace JetBrains.ForTea.RiderPlugin.ProtocolAware.Action
{
	public abstract class T4FileBasedActionBase : IExecutableAction
	{
		public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate)
		{
			var solution = FindSolution(context);
			if (solution == null) return false;
			var psiSourceFile = context.GetData(PsiDataConstants.SOURCE_FILE);
			if (psiSourceFile == null) return false;
			return DoUpdate(psiSourceFile, solution);
		}

		protected virtual bool DoUpdate([NotNull] IPsiSourceFile file, [NotNull] ISolution solution) => true;

		public abstract void Execute(IDataContext context, DelegateExecute nextExecute);

		[CanBeNull]
		protected static IT4File FindT4File([NotNull] IDataContext context, [NotNull] ISolution solution)
		{
			solution.GetComponent<IPsiFiles>().CommitAllDocuments();
			return context
				.GetData(PsiDataConstants.SOURCE_FILE)
				?.GetPsiFiles<T4Language>()
				.OfType<IT4File>()
				.SingleItem();
		}

		[CanBeNull]
		protected static ISolution FindSolution([NotNull] IDataContext context) => context
			.GetData(ProjectModelDataConstants.SOLUTION);
	}
}
