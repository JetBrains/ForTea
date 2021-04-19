using GammaJul.ForTea.Core.Tree;

namespace GammaJul.ForTea.Core.Parsing.Parser
{
	public readonly struct T4NodeCloningResult
	{
		public bool ShouldContinueRecursiveDescent { get; }
		public IT4TreeNode CurrentClone { get; }

		public T4NodeCloningResult(bool shouldContinueRecursiveDescent, IT4TreeNode currentClone)
		{
			ShouldContinueRecursiveDescent = shouldContinueRecursiveDescent;
			CurrentClone = currentClone;
		}
	}
}
