using System.Text;
using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Text;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Parsing {

	/// <summary>A factory for T4 composite elements.</summary>
	public static class T4ElementFactory {

		/// <summary>Creates a new statement block (&lt;# ... #&gt;).</summary>
		/// <param name="code">The code that will be contained in the block.</param>
		/// <returns>A new instance of <see cref="IT4StatementBlock"/>.</returns>
		[NotNull]
		public static IT4StatementBlock CreateStatementBlock([CanBeNull] string code)
			=> (IT4StatementBlock) CreateTreeAndGetFirstChild("<#" + code + "#>");

		/// <summary>Creates a new feature block (&lt;#+ ... #&gt;).</summary>
		/// <param name="code">The code that will be contained in the block.</param>
		/// <returns>A new instance of <see cref="IT4FeatureBlock"/>.</returns>
		[NotNull]
		public static IT4FeatureBlock CreateFeatureBlock([CanBeNull] string code)
			=> (IT4FeatureBlock) CreateTreeAndGetFirstChild("<#+" + code + "#>");

		/// <summary>Creates a new directive (&lt;#@ ... #&gt;).</summary>
		/// <param name="directiveName">Name of the directive.</param>
		/// <param name="attributes">The directive attributes.</param>
		/// <returns>A new instance of <see cref="IT4Directive"/>.</returns>
		[NotNull]
		public static IT4Directive CreateDirective([CanBeNull] string directiveName, [CanBeNull] params Pair<string, string>[] attributes) {
			var builder = new StringBuilder("<#@ ");
			builder.Append(directiveName);
			if (attributes != null) {
				foreach (var pair in attributes)
					builder.AppendFormat(" {0}=\"{1}\"", pair.First, pair.Second);
			}
			builder.Append(" #>");
			return (IT4Directive) CreateTreeAndGetFirstChild(builder.ToString());
		}

		public static ITreeNode CreateAttributeValue([NotNull] string text) =>
			CreateDirectiveAttribute("attributeName", text).Value;

		/// <summary>Creates a new directive attribute.</summary>
		/// <param name="name">The name of the attribute.</param>
		/// <param name="value">The value of the attribute.</param>
		/// <returns>A new instance of <see cref="IT4DirectiveAttribute"/>.</returns>
		[NotNull]
		public static IT4DirectiveAttribute CreateDirectiveAttribute([CanBeNull] string name, [CanBeNull] string value) {
			var directive = CreateDirective("dummy", Pair.Of(name, value));
			return directive.Attributes.First();
		}

		[NotNull]
		private static ITreeNode CreateTreeAndGetFirstChild([NotNull] string text) {
			LanguageService languageService = T4Language.Instance.LanguageService();
			Assertion.AssertNotNull(languageService, "languageService != null");

			ILexer lexer = languageService.GetPrimaryLexerFactory().CreateLexer(new StringBuffer(text));
			IParser parser = languageService.CreateParser(lexer, null, null);
			IFile file = parser.ParseFile();
			Assertion.AssertNotNull(file, "file != null");
			Assertion.AssertNotNull(file.FirstChild, "file.FirstChild != null");
			return file.FirstChild;
		}

	}

}
