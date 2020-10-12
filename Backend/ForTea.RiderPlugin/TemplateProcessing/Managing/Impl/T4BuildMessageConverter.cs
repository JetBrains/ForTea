using System;
using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Interrupt;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ForTea.RiderPlugin.Model;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Host.Features.ProjectModel.View;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util.dataStructures;
using Microsoft.CodeAnalysis;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing.Impl
{
	[SolutionComponent]
	public sealed class T4BuildMessageConverter : IT4BuildMessageConverter
	{
		[CanBeNull]
		private ProjectModelViewHost Host { get; }

		public T4BuildMessageConverter([NotNull] ISolution solution) =>
			Host = solution.TryGetComponent<ProjectModelViewHost>();

		public T4BuildResult ToT4BuildResult(ICollection<Diagnostic> diagnostics, [NotNull] IT4File file)
		{
			int id = GetProjectId(file.LogicalPsiSourceFile.ToProjectFile().NotNull());
			var kind = ToT4BuildResultKind(diagnostics);
			var messages = diagnostics.Select(diagnostic => ToT4BuildMessage(diagnostic, id)).ToList();
			return new T4BuildResult(kind, messages);
		}

		private int GetProjectId([NotNull] IProjectFile file) => Host?.GetIdByItem(file.GetProject().NotNull()) ?? 0;

		public T4BuildResult ToT4BuildResult(T4OutputGenerationException exception) =>
			ToT4BuildResult(exception.FailureDatum.AsEnumerable());

		[NotNull]
		private T4BuildResult ToT4BuildResult([NotNull] IEnumerable<T4FailureRawData> data) =>
			new T4BuildResult(T4BuildResultKind.HasErrors, ToT4BuildMessages(data));

		public List<T4BuildMessage> ToT4BuildMessages(IEnumerable<T4FailureRawData> rawData)
		{
			var messages = new List<T4BuildMessage>();
			foreach (var data in rawData)
			{
				var location = new T4Location(data.Line, data.Column);
				int projectId = GetProjectId(data.ProjectFile);
				string fullPath = data.ProjectFile.Location.FullPath;
				string message = data.Message;
				const T4BuildMessageKind kind = T4BuildMessageKind.Error;
				messages.Add(new T4BuildMessage(kind, "Error", location, message, projectId, fullPath));
			}

			return messages;
		}

		public T4BuildResult FatalError()
		{
			var location = new T4Location(-1, -1);
			const string content = "Fatal internal error";
			var message = new T4BuildMessage(T4BuildMessageKind.Error, "Error", location, content, -1, null);
			var messages = new List<T4BuildMessage> {message};
			return new T4BuildResult(T4BuildResultKind.HasErrors, messages);
		}

		public T4BuildResult SyntaxErrors(IEnumerable<ITreeNode> nodes) =>
			ToT4BuildResult(nodes.Select(node => T4FailureRawData.FromElement(node, "Syntax error")));

		private static T4BuildResultKind ToT4BuildResultKind([NotNull, ItemNotNull] ICollection<Diagnostic> diagnostics)
		{
			if (diagnostics.Any(it => it.Severity == DiagnosticSeverity.Error)) return T4BuildResultKind.HasErrors;
			if (diagnostics.Any(it => it.Severity == DiagnosticSeverity.Warning)) return T4BuildResultKind.HasWarnings;
			return T4BuildResultKind.Successful;
		}

		[NotNull]
		private T4BuildMessage ToT4BuildMessage([NotNull] Diagnostic diagnostic, int projectId)
		{
			var kind = ToT4BuildMessageKind(diagnostic.Severity);
			var mappedSpan = diagnostic.Location.GetMappedLineSpan();
			var start = mappedSpan.StartLinePosition;
			var location = new T4Location(start.Line, start.Character);
			string path = mappedSpan.Path;
			string message = diagnostic.GetMessage();
			string id = diagnostic.Id;
			return new T4BuildMessage(kind, id, location, message, projectId, path);
		}

		private T4BuildMessageKind ToT4BuildMessageKind(DiagnosticSeverity severity)
		{
			switch (severity)
			{
				case DiagnosticSeverity.Hidden:
					return T4BuildMessageKind.Message;
				case DiagnosticSeverity.Info:
					return T4BuildMessageKind.Message;
				case DiagnosticSeverity.Warning:
					return T4BuildMessageKind.Warning;
				case DiagnosticSeverity.Error:
					return T4BuildMessageKind.Error;
				default:
					throw new ArgumentOutOfRangeException(nameof(severity), severity, null);
			}
		}
	}
}
