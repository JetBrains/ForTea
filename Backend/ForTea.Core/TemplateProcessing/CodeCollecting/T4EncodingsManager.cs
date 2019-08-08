using System;
using System.Text;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Interrupt;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting
{
	[SolutionComponent]
	public class T4EncodingsManager
	{
		[NotNull]
		private T4DirectiveInfoManager Manager { get; }

		public T4EncodingsManager([NotNull] T4DirectiveInfoManager manager) => Manager = manager;

		[NotNull]
		public string FindEncoding([NotNull] IT4Directive directive, [NotNull] IT4CodeGenerationInterrupter interrupter)
		{
			Assertion.Assert(directive.IsSpecificDirective(Manager.Output),
				"directive.IsSpecificDirective(Manager.Output)");
			var attribute = directive.GetAttribute(Manager.Output.EncodingAttribute.Name);
			var value = attribute?.GetValueToken();
			if (value == null) return "utf-8";
			string rawEncoding = value.GetText();
			if (IsCodePage(rawEncoding)) return rawEncoding;
			if (IsEncodingName(rawEncoding)) return rawEncoding;
			interrupter.InterruptAfterProblem(T4FailureRawData.FromElement(value, "Unknown encoding"));
			return "utf-8";
		}

		public static bool IsCodePage([NotNull] string rawEncoding)
		{
			if (!int.TryParse(rawEncoding, out int codePage)) return false;
			try
			{
				// ReSharper disable once ReturnValueOfPureMethodIsNotUsed
				Encoding.GetEncoding(codePage);
				return true;
			}
			catch (ArgumentException)
			{
				return false;
			}
			catch (NotSupportedException)
			{
				return false;
			}
		}

		public static bool IsEncodingName([NotNull] string rawEncoding)
		{
			try
			{
				// ReSharper disable once ReturnValueOfPureMethodIsNotUsed
				Encoding.GetEncoding(rawEncoding);
				return true;
			}
			catch (ArgumentException)
			{
				return false;
			}
		}
	}
}
