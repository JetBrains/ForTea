using System.Linq;
using GammaJul.ForTea.Core.Psi.Resolve.Macros.Impl;
using GammaJul.ForTea.Core.Tree;
using GammaJul.ForTea.Core.Tree.Impl;
using JetBrains.Annotations;

namespace JetBrains.ForTea.Tests.Psi
{
	public static class T4CloningParserTestUtils
	{
		public static void InitializeResolvePaths([NotNull] IT4File file)
		{
			foreach (var directive in file.BlocksEnumerable.OfType<T4DirectiveWithPathBase>())
			{
				directive.InitializeResolvedPath(new T4ResolvedPath("dummy", null, null));
			}
		}
	}
}
