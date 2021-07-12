using System.ComponentModel.Composition;
using System.Windows.Media;
using GammaJul.ForTea.Core.Daemon.Attributes;
using JetBrains.Platform.VisualStudio.SinceVs10.TextControl.Markup.FormatDefinitions;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace JetBrains.ForTea.ReSharperPlugin.Daemon.Attributes
{
	#region block tag
	[ClassificationType(ClassificationTypeNames = Name)]
	[Order(
		After = VsSyntaxPriorityClassificationDefinition.Name,
		Before = VsAnalysisPriorityClassificationDefinition.Name
	)]
	[Export(typeof(EditorFormatDefinition))]
	[Name(Name)]
	[System.ComponentModel.DisplayName(Name)]
	[UserVisible(true)]
	internal sealed class BlockTagClassificationFormatDefinition : ClassificationFormatDefinition
	{
		private const string Name = T4HighlightingAttributeIds.BLOCK_TAG;

		public BlockTagClassificationFormatDefinition()
		{
			DisplayName = Name;
			BackgroundColor = Color.FromRgb(0xFB, 0xFB, 0x64);
			ForegroundColor = Color.FromRgb(0x00, 0x00, 0x00);
		}

		[Export, Name(Name), BaseDefinition("formal language")]
		internal ClassificationTypeDefinition ClassificationTypeDefinition;
	}
	#endregion

	#region directive
	[ClassificationType(ClassificationTypeNames = Name)]
	[Order(
		After = VsSyntaxPriorityClassificationDefinition.Name,
		Before = VsAnalysisPriorityClassificationDefinition.Name
	)]
	[Export(typeof(EditorFormatDefinition))]
	[Name(Name)]
	[System.ComponentModel.DisplayName(Name)]
	[UserVisible(true)]
	internal sealed class DirectiveClassificationFormatDefinition : ClassificationFormatDefinition
	{
		private const string Name = T4HighlightingAttributeIds.DIRECTIVE;

		public DirectiveClassificationFormatDefinition()
		{
			DisplayName = Name;
			ForegroundColor = Color.FromRgb(0xEE, 0x82, 0xEE);
		}

		[Export, Name(Name), BaseDefinition("formal language")]
		internal ClassificationTypeDefinition ClassificationTypeDefinition;
	}
	#endregion

	#region directive attribute
	[ClassificationType(ClassificationTypeNames = Name)]
	[Order(
		After = VsSyntaxPriorityClassificationDefinition.Name,
		Before = VsAnalysisPriorityClassificationDefinition.Name
	)]
	[Export(typeof(EditorFormatDefinition))]
	[Name(Name)]
	[System.ComponentModel.DisplayName(Name)]
	[UserVisible(true)]
	internal sealed class DirectiveAttributeClassificationFormatDefinition : ClassificationFormatDefinition
	{
		private const string Name = T4HighlightingAttributeIds.DIRECTIVE_ATTRIBUTE;

		public DirectiveAttributeClassificationFormatDefinition()
		{
			DisplayName = Name;
			ForegroundColor = Color.FromRgb(0x56, 0x9C, 0xD6);
		}

		[Export, Name(Name), BaseDefinition("formal language")]
		internal ClassificationTypeDefinition ClassificationTypeDefinition;
	}
	#endregion

	#region attribute value
	[ClassificationType(ClassificationTypeNames = Name)]
	[Order(
		After = VsSyntaxPriorityClassificationDefinition.Name,
		Before = VsAnalysisPriorityClassificationDefinition.Name
	)]
	[Export(typeof(EditorFormatDefinition))]
	[Name(Name)]
	[System.ComponentModel.DisplayName(Name)]
	[UserVisible(true)]
	internal sealed class AttributeValueClassificationFormatDefinition : ClassificationFormatDefinition
	{
		private const string Name = T4HighlightingAttributeIds.ATTRIBUTE_VALUE;

		public AttributeValueClassificationFormatDefinition()
		{
			DisplayName = Name;
			ForegroundColor = Color.FromRgb(0x9C, 0x58, 0x00);
		}

		[Export, Name(Name), BaseDefinition("formal language")]
		internal ClassificationTypeDefinition ClassificationTypeDefinition;
	}
	#endregion

	#region macro
	[ClassificationType(ClassificationTypeNames = Name)]
	[Order(
		After = VsSyntaxPriorityClassificationDefinition.Name,
		Before = VsAnalysisPriorityClassificationDefinition.Name
	)]
	[Export(typeof(EditorFormatDefinition))]
	[Name(Name)]
	[System.ComponentModel.DisplayName(Name)]
	[UserVisible(true)]
	internal sealed class MacroClassificationFormatDefinition : ClassificationFormatDefinition
	{
		private const string Name = T4HighlightingAttributeIds.MACRO;

		public MacroClassificationFormatDefinition()
		{
			DisplayName = Name;
			ForegroundColor = Color.FromRgb(0x56, 0x9C, 0xD6);
		}

		[Export, Name(Name), BaseDefinition("formal language")]
		internal ClassificationTypeDefinition ClassificationTypeDefinition;
	}
	#endregion

	#region environment variable
	[ClassificationType(ClassificationTypeNames = Name)]
	[Order(
		After = VsSyntaxPriorityClassificationDefinition.Name,
		Before = VsAnalysisPriorityClassificationDefinition.Name
	)]
	[Export(typeof(EditorFormatDefinition))]
	[Name(Name)]
	[System.ComponentModel.DisplayName(Name)]
	[UserVisible(true)]
	internal sealed class EnvironmentVariableClassificationFormatDefinition : ClassificationFormatDefinition
	{
		private const string Name = T4HighlightingAttributeIds.ENVIRONMENT_VARIABLE;

		public EnvironmentVariableClassificationFormatDefinition()
		{
			DisplayName = Name;
			ForegroundColor = Color.FromRgb(0xB5, 0xCE, 0xA8);
		}

		[Export, Name(Name), BaseDefinition("formal language")]
		internal ClassificationTypeDefinition ClassificationTypeDefinition;
	}
	#endregion
}
