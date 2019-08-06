using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.Rider.Model;
using Microsoft.CodeAnalysis;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing
{
	public static class T4BuildMessageUtils
	{
		public static T4BuildResult ToT4BuildResult([NotNull, ItemNotNull] this ICollection<Diagnostic> diagnostics) =>
			new T4BuildResult(diagnostics.ToT4BuildResultKind(), diagnostics.Select(ToT4BuildMessage).ToList());

		private static T4BuildResultKind ToT4BuildResultKind(
			[NotNull, ItemNotNull] this ICollection<Diagnostic> diagnostics
		)
		{
			if (diagnostics.Any(it => it.Severity == DiagnosticSeverity.Error)) return T4BuildResultKind.HasErrors;
			if (diagnostics.Any(it => it.Severity == DiagnosticSeverity.Warning)) return T4BuildResultKind.HasWarnings;
			return T4BuildResultKind.Successful;
		}

		[NotNull]
		private static T4BuildMessage ToT4BuildMessage([NotNull] Diagnostic diagnostic)
		{
			var kind = diagnostic.Severity.ToT4BuildMessageKind();
			var start = diagnostic.Location.GetMappedLineSpan().StartLinePosition;
			var location = new T4Location(start.Line, start.Character);
			return new T4BuildMessage(kind, diagnostic.Id, location, diagnostic.GetMessage());
		}

		private static T4BuildMessageKind ToT4BuildMessageKind(this DiagnosticSeverity severity)
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
