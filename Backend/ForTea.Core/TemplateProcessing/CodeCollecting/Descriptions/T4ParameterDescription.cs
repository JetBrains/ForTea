using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Metadata.Reader.Impl;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Descriptions
{
	public sealed class T4ParameterDescription
	{
		[NotNull]
		private ITreeNode TypeToken { get; }

		[NotNull]
		private ITreeNode NameToken { get; }

		[NotNull]
		public string FieldNameString { get; }

		[NotNull]
		public string PropertyNameString { get; }

		[NotNull]
		public string TypeString { get; }

		[NotNull]
		public string TypeFqnString { get; }

		private T4ParameterDescription(
			[NotNull] ITreeNode typeToken,
			[NotNull] ITreeNode nameToken,
			[NotNull] string nameString
		)
		{
			TypeToken = typeToken;
			NameToken = nameToken;
			FieldNameString = $"_{nameString}Field";
			PropertyNameString = nameString;
			TypeString = GetTypeString();
			TypeFqnString = GetTypeFqnString();
		}

		public void AppendName([NotNull] T4CSharpCodeGenerationResult result)
		{
			if (CSharpLexer.IsKeyword(NameToken.GetText())) result.Append("@");
			result.AppendMapped(NameToken);
		}

		public void AppendTypeMapped([NotNull] T4CSharpCodeGenerationResult result)
		{
			string typeText = TypeToken.GetText();
			string keyword = CSharpTypeFactory.GetTypeKeyword(new ClrTypeName(typeText));
			if (keyword != null)
			{
				result.Append(keyword);
				return;
			}

			result.Append("global::");
			if (CSharpLexer.IsKeyword(typeText)) result.Append("@");
			result.AppendMapped(TypeToken);
		}

		[NotNull]
		private string GetTypeString()
		{
			string keyword = CSharpTypeFactory.GetTypeKeyword(new ClrTypeName(TypeToken.GetText()));
			if (keyword != null)
			{
				return keyword;
			}

			return "global::" + GetTypeFqnString();
		}

		[NotNull]
		private string GetTypeFqnString()
		{
			string typeText = TypeToken.GetText();
			if (CSharpLexer.IsKeyword(typeText)) return $"@{typeText}";
			return typeText;
		}

		public void AppendType([NotNull] T4CSharpCodeGenerationResult result) => result.Append(TypeString);

		[CanBeNull]
		public static T4ParameterDescription FromDirective([NotNull] IT4Directive directive)
		{
			var typeToken = directive.GetAttributeValueToken(T4DirectiveInfoManager.Parameter.TypeAttribute.Name);
			string typeText = typeToken?.GetText();
			var nameToken = directive.GetAttributeValueToken(T4DirectiveInfoManager.Parameter.NameAttribute.Name);
			string nameText = nameToken?.GetText();
			if (string.IsNullOrEmpty(typeText)) return null;
			if (string.IsNullOrEmpty(nameText)) return null;
			return new T4ParameterDescription(typeToken, nameToken, nameText);
		}
	}
}
