using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.State
{
	public interface IT4InfoCollectorState
	{
		[CanBeNull]
		string Produce(ITreeNode lookahead);

		[CanBeNull]
		string ProduceBeforeEof();

		[CanBeNull]
		IT4TreeNode FirstNode { get; }

		[NotNull]
		IT4InfoCollectorState GetNextState([NotNull] IT4TreeNode element);

		bool FeatureStarted { get; }
		void ConsumeToken([NotNull] IT4Token token);
	}
}
