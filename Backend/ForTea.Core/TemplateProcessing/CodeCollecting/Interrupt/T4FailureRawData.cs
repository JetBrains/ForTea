using System.Linq;
using GammaJul.ForTea.Core.Parsing.Ranges;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Threading;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Interrupt
{
	public readonly struct T4FailureRawData
	{
		public int Line { get; }
		public int Column { get; }

		[NotNull]
		private IPsiSourceFile PsiSourceFile { get; }

		[NotNull]
		public IProjectFile ProjectFile => PsiSourceFile.ToProjectFile().NotNull();

		[NotNull]
		public string Message { get; }

		private T4FailureRawData(int line, int column, [NotNull] IPsiSourceFile sourceFile, [NotNull] string message)
		{
			Line = line;
			Column = column;
			PsiSourceFile = sourceFile;
			Message = message;
		}

		public static T4FailureRawData FromElement([NotNull] ITreeNode node, [NotNull] string message)
		{
			node.GetSolution().Locks.AssertReadAccessAllowed();
			var file = FindSuitableSourceFile(node);
			var offset = T4UnsafeManualRangeTranslationUtil.GetDocumentStartOffset(node);
			var coords = offset.Document.GetCoordsByOffset(offset.Offset);
			return new T4FailureRawData((int) coords.Line, (int) coords.Column, file, message);
		}

		[NotNull]
		private static IPsiSourceFile FindSuitableSourceFile([NotNull] ITreeNode node) => node
			.GetParentsOfType<IT4FileLikeNode>()
			.Select(file => file.LogicalPsiSourceFile)
			.FirstOrDefault(sourceFile => sourceFile.ToProjectFile() != null)
		    ?? node
			    .GetParentOfType<IT4FileLikeNode>()
			    .NotNull()
			    .LogicalPsiSourceFile;
	}
}
