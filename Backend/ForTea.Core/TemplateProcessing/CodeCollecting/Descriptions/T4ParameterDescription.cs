using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Descriptions
{
	public sealed class T4ParameterDescription : T4ElementDescriptionBase
	{
		[NotNull]
		public ITreeNode TypeToken { get; }

		[NotNull]
		public ITreeNode NameToken { get; }

		[NotNull]
		public string TypeString { get; }

		[NotNull]
		public string NameString { get; }

		[NotNull]
		public string FieldNameString { get; }

		private T4ParameterDescription(
			[NotNull] ITreeNode typeToken,
			[NotNull] ITreeNode nameToken,
			[NotNull] string typeString,
			[NotNull] string nameString
		) : base(typeToken.GetSourceFile())
		{
			TypeToken = typeToken;
			NameToken = nameToken;
			TypeString = typeString.EscapeKeyword();
			NameString = nameString.EscapeKeyword();
			FieldNameString = $"_{nameString}Field";
		}

		[CanBeNull]
		public static T4ParameterDescription FromDirective([NotNull] IT4Directive directive)
		{
			var typeToken = directive.GetAttributeValueToken(T4DirectiveInfoManager.Parameter.TypeAttribute.Name);
			string typeText = typeToken?.GetText();
			var nameToken = directive.GetAttributeValueToken(T4DirectiveInfoManager.Parameter.NameAttribute.Name);
			string nameText = nameToken?.GetText();
			if (string.IsNullOrEmpty(typeText)) return null;
			if (string.IsNullOrEmpty(nameText)) return null;
			return new T4ParameterDescription(typeToken, nameToken, typeText, nameText);
		}
	}
}
