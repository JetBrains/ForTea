using System;
using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Interrupt;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Host.Features.ProjectModel.View;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Rider.Model;
using Microsoft.CodeAnalysis;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing.Impl
{
	[SolutionComponent]
	public class T4BuildMessageConverter : IT4BuildMessageConverter
	{
		[NotNull]
		private ProjectModelViewHost Host { get; }

		public T4BuildMessageConverter([NotNull] ProjectModelViewHost host) => Host = host;

		public T4BuildResult ToT4BuildResult(ICollection<Diagnostic> diagnostics, IT4File file)
		{
			int id = GetProjectId(file);
			var kind = ToT4BuildResultKind(diagnostics);
			var messages = diagnostics.Select(diagnostic => ToT4BuildMessage(diagnostic, id)).ToList();
			return new T4BuildResult(kind, messages);
		}

		private int GetProjectId([NotNull] IT4File file) => Host.GetIdByItem(file.GetProject().NotNull());

		public T4BuildResult ToT4BuildResult(T4OutputGenerationException exception) =>
			ToT4BuildResult(exception.FailureData);

		public T4PreprocessingResult ToT4PreprocessingResult(T4OutputGenerationException exception) =>
			new T4PreprocessingResult(false, ToT4BuildMessage(exception.FailureData));

		private T4BuildResult ToT4BuildResult(T4FailureRawData data)
		{
			var message = ToT4BuildMessage(data);
			var messages = new List<T4BuildMessage> {message};
			return new T4BuildResult(T4BuildResultKind.HasErrors, messages);
		}

		private T4BuildMessage ToT4BuildMessage(T4FailureRawData data)
		{
			var location = new T4Location(data.Line, data.Column);
			int projectId = GetProjectId(data.File);
			var message = new T4BuildMessage(T4BuildMessageKind.Error, "Error", location, data.Message, projectId);
			return message;
		}

		public T4BuildResult FatalError()
		{
			var location = new T4Location(-1, -1);
			var message = new T4BuildMessage(T4BuildMessageKind.Error, "Error", location, "Fatal internal error", -1);
			var messages = new List<T4BuildMessage> {message};
			return new T4BuildResult(T4BuildResultKind.HasErrors, messages);
		}

		public T4BuildResult SyntaxError(ITreeNode node) =>
			ToT4BuildResult(T4FailureRawData.FromElement(node, "Syntax error"));

		private T4BuildResultKind ToT4BuildResultKind([NotNull, ItemNotNull] ICollection<Diagnostic> diagnostics)
		{
			if (diagnostics.Any(it => it.Severity == DiagnosticSeverity.Error)) return T4BuildResultKind.HasErrors;
			if (diagnostics.Any(it => it.Severity == DiagnosticSeverity.Warning)) return T4BuildResultKind.HasWarnings;
			return T4BuildResultKind.Successful;
		}

		[NotNull]
		private T4BuildMessage ToT4BuildMessage([NotNull] Diagnostic diagnostic, int projectId)
		{
			var kind = ToT4BuildMessageKind(diagnostic.Severity);
			var start = diagnostic.Location.GetMappedLineSpan().StartLinePosition;
			var location = new T4Location(start.Line, start.Character);
			return new T4BuildMessage(kind, diagnostic.Id, location, diagnostic.GetMessage(), projectId);
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
