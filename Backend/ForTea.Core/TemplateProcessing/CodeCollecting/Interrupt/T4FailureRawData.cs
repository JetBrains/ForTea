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
		public IProjectFile ProjectFile { get; }

		[NotNull]
		public string Message { get; }

		private T4FailureRawData(int line, int column, [NotNull] IProjectFile projectFile, [NotNull] string message)
		{
			Line = line;
			Column = column;
			ProjectFile = projectFile;
			Message = message;
		}

		public static T4FailureRawData FromElement([NotNull] ITreeNode node, [NotNull] string message)
		{
			var file = node.GetParentOfType<IT4FileLikeNode>().NotNull();
			file.GetSolution().Locks.AssertReadAccessAllowed();
			var projectFile = file.LogicalPsiSourceFile.ToProjectFile().NotNull();
			var offset = T4UnsafeManualRangeTranslationUtil.GetDocumentStartOffset(node);
			var coords = offset.Document.GetCoordsByOffset(offset.Offset);
			return new T4FailureRawData((int) coords.Line, (int) coords.Column, projectFile, message);
		}
	}
}
