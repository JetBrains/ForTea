using System;
using System.Linq;
using System.Text;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Interrupt;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ProjectModel;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting
{
	[SolutionComponent]
	public class T4EncodingsManager
	{
		[CanBeNull]
		public string FindEncoding([NotNull] IT4OutputDirective directive, [NotNull] IT4CodeGenerationInterrupter interrupter)
		{
			var attribute = directive.GetAttributes(T4DirectiveInfoManager.Output.EncodingAttribute).FirstOrDefault();
			var value = attribute?.Value;
			if (value == null) return null;
			string rawEncoding = value.GetText();
			if (IsCodePage(rawEncoding)) return rawEncoding; // Insert unquoted
			if (IsEncodingName(rawEncoding)) return $"\"{rawEncoding}\"";
			interrupter.InterruptAfterProblem(T4FailureRawData.FromElement(value, "Unknown encoding"));
			return null;
		}

		[NotNull]
		public static string GetEncoding([NotNull] IT4File file)
		{
			var sourceFile = file.GetSourceFile();
			if (sourceFile == null) return Encoding.UTF8.CodePage.ToString();
			return sourceFile.Document.Encoding.CodePage.ToString();
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
