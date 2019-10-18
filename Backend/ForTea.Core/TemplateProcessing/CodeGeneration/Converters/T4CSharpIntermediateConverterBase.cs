using System.Collections.Generic;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Descriptions;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.Util;
using JetBrains.Util.dataStructures.TypedIntrinsics;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters
{
	public abstract class T4CSharpIntermediateConverterBase : IT4ElementAppendFormatProvider
	{
		[NotNull] public const string GeneratedClassNameString = "GeneratedTextTransformation";
		[NotNull] public const string GeneratedBaseClassNameString = "TextTransformation";
		[NotNull] internal const string TransformTextMethodName = "TransformText";

		[NotNull] private const string ToStringInstanceHelperResource =
			"GammaJul.ForTea.Core.Resources.ToStringInstanceHelper.cs";

		[NotNull]
		protected T4CSharpCodeGenerationIntermediateResult IntermediateResult { get; }

		[NotNull]
		protected T4CSharpCodeGenerationResult Result { get; }

		[NotNull]
		protected IT4File File { get; }

		protected T4CSharpIntermediateConverterBase(
			[NotNull] T4CSharpCodeGenerationIntermediateResult intermediateResult,
			[NotNull] IT4File file
		)
		{
			IntermediateResult = intermediateResult;
			File = file;
			Result = new T4CSharpCodeGenerationResult(File);
		}

		[NotNull]
		public T4CSharpCodeGenerationResult Convert()
		{
			AppendGeneratedMessage();
			string ns = GetNamespace();
			AppendNamespacePrefix();
			bool hasNamespace = !string.IsNullOrEmpty(ns);
			if (hasNamespace)
			{
				Result.AppendLine($"namespace {ns}");
				Result.AppendLine("{");
				PushIndent();
				AppendNamespaceContents();
				PopIndent();
				Result.AppendLine("}");
			}
			else
			{
				AppendNamespaceContents();
			}

			return Result;
		}

		protected virtual void AppendNamespacePrefix()
		{
		}

		private void AppendNamespaceContents()
		{
			AppendImports();
			AppendClasses();
		}

		protected virtual void AppendClasses()
		{
			AppendClass();
			AppendIndent();
			Result.AppendLine();
			AppendBaseClass();
		}

		[CanBeNull]
		protected string GetNamespace() => File.GetSourceFile()?.Properties.GetDefaultNamespace();

		private void AppendImports()
		{
			if (ShouldAppendPragmaDirectives)
			{
				AppendIndent();
				Result.AppendLine("#pragma warning disable 8019");
			}

			AppendIndent();
			Result.AppendLine("using System;");
			if (IntermediateResult.HasHost)
			{
				AppendIndent();
				Result.AppendLine("using System.CodeDom.Compiler;");
			}

			if (ShouldAppendPragmaDirectives)
			{
				AppendIndent();
				Result.AppendLine("#pragma warning restore 8019");
			}

			foreach (var description in IntermediateResult.ImportDescriptions)
			{
				description.AppendContent(Result, this, File.GetSourceFile());
			}
		}

		protected virtual void AppendClass()
		{
			AppendSyntheticAttribute();
			AppendIndent();
			Result.Append($"public partial class {GeneratedClassName} : ");
			AppendBaseClassName();
			Result.AppendLine();
			AppendIndent();
			Result.AppendLine("{");
			PushIndent();
			if (IntermediateResult.HasHost) AppendHost();
			AppendTransformMethod();
			foreach (var description in IntermediateResult.FeatureDescriptions)
			{
				description.AppendContent(Result, this, File.GetSourceFile());
			}

			AppendParameterDeclarations(IntermediateResult.ParameterDescriptions);
			AppendTemplateInitialization(IntermediateResult.ParameterDescriptions);
			PopIndent();
			AppendIndent();
			Result.AppendLine("}");
		}

		private void AppendParameterDeclarations(
			[NotNull, ItemNotNull] IEnumerable<T4ParameterDescription> descriptions
		)
		{
			foreach (var description in descriptions)
			{
				AppendFieldDeclaration(description);
				AppendParameterDeclaration(description);
			}
		}

		private void AppendFieldDeclaration([NotNull] T4ParameterDescription description)
		{
			AppendSyntheticAttribute();
			AppendIndent();
			Result.Append("private global::");
			Result.Append(description.TypeString);
			Result.Append(" ");
			Result.Append(description.FieldNameString);
			Result.AppendLine(";");
		}

		private void AppendTemplateInitialization(
			[NotNull, ItemNotNull] IReadOnlyCollection<T4ParameterDescription> descriptions
		)
		{
			if (descriptions.IsEmpty()) return;
			AppendSyntheticAttribute();
			AppendIndent();
			Result.AppendLine("public void Initialize()");
			AppendIndent();
			Result.AppendLine("{");
			PushIndent();
			AppendParameterInitialization(descriptions);
			PopIndent();
			AppendIndent();
			Result.AppendLine("}");
		}

		protected virtual void AppendTransformMethod()
		{
			AppendSyntheticAttribute();
			AppendIndent();
			Result.Append("public ");
			Result.Append(IntermediateResult.HasBaseClass ? "override" : "virtual");
			Result.AppendLine($" string {TransformTextMethodName}()");
			AppendIndent();
			Result.AppendLine("{");
			PushIndent();
			AppendIndent();
			Result.AppendLine();
			foreach (var description in IntermediateResult.TransformationDescriptions)
			{
				description.AppendContent(Result, this, File.GetSourceFile());
			}

			AppendIndent();
			Result.AppendLine("return this.GenerationEnvironment.ToString();");
			PopIndent();
			AppendIndent();
			Result.AppendLine("}");
		}

		private void AppendBaseClassName()
		{
			if (IntermediateResult.HasBaseClass) Result.Append(IntermediateResult.CollectedBaseClass);
			else Result.Append(GeneratedBaseClassName);
		}

		protected void AppendBaseClass()
		{
			string resource = !IntermediateResult.HasBaseClass ? BaseClassResourceName : ToStringInstanceHelperResource;
			var provider = new T4TemplateResourceProvider(resource);
			Result.Append(provider.ProcessResource(GeneratedBaseClassName));
		}

		protected abstract void AppendHost();
		protected abstract void AppendParameterDeclaration([NotNull] T4ParameterDescription description);

		[NotNull]
		protected abstract string BaseClassResourceName { get; }

		[NotNull]
		protected virtual string GeneratedClassName => GeneratedClassNameString;

		[NotNull]
		protected virtual string GeneratedBaseClassName => GeneratedBaseClassNameString;

		protected abstract void AppendSyntheticAttribute();

		protected abstract void AppendParameterInitialization(
			[NotNull, ItemNotNull] IReadOnlyCollection<T4ParameterDescription> descriptions);

		protected virtual void AppendGeneratedMessage()
		{
		}

		protected virtual bool ShouldAppendPragmaDirectives => false;

		#region Indentation
		protected int CurrentIndent { get; private set; }
		protected void PushIndent() => CurrentIndent += 1;
		protected void PopIndent() => CurrentIndent -= 1;
		protected void AppendIndent() => AppendIndent(CurrentIndent);
		protected abstract void AppendIndent(int size);
		#endregion Indentation

		#region IT4ElementAppendFormatProvider
		[NotNull]
		public virtual string ToStringConversionPrefix => "this.ToStringHelper.ToStringWithCulture(";

		[NotNull]
		public string ToStringConversionSuffix => ")";

		[NotNull]
		public string ExpressionWritingPrefix => "this.Write(";

		[NotNull]
		public string ExpressionWritingSuffix => ");";

		public abstract string CodeCommentStart { get; }
		public abstract string CodeCommentEnd { get; }
		public abstract string Indent { get; }
		public abstract bool ShouldBreakExpressionWithLineDirective { get; }
		public abstract void AppendCompilationOffset(T4CSharpCodeGenerationResult destination, Int32<DocColumn> offset);
		public abstract void AppendMappedIfNeeded(T4CSharpCodeGenerationResult destination, IT4Code code);
		#endregion IT4ElementAppendFormatProvider
	}
}
