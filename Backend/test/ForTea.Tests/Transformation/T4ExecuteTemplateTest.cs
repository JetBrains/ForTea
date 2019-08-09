using GammaJul.ForTea.Core.Psi;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;

namespace JetBrains.ForTea.Tests.Transformation
{
	[TestFileExtension(T4ProjectFileType.MainExtension)]
	[Ignore("Template execution has moved to frontend")]
	public sealed class T4ExecuteTemplateTest : ContextActionExecuteTestBase<IContextAction>
	{
		protected override string ExtraPath => @"TemplateProcessing";

		[TestCase("PlainText")]
		[TestCase("DefaultExtension")]
		[TestCase("DefaultDirectives")]
		[TestCase("PushIndent")]
		[TestCase("NewlinesInFeature")]
		[TestCase("TextInFeature")]
		[TestCase("ExtensionWithoutDot")]
		[TestCase("LineBreakMess")]
		public void TestExecuteTemplate(string name) => DoOneTest(name);
	}
}
