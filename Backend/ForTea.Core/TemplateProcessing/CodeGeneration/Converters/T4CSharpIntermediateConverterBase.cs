using System.Collections.Generic;
using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Descriptions;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters
{
	public abstract class T4CSharpIntermediateConverterBase : IT4ElementAppendFormatProvider
	{
		[NotNull] public const string GeneratedClassNameString = "GeneratedTextTransformation";
		[NotNull] public const string GeneratedBaseClassNameString = "TextTransformation";
		[NotNull] internal const string TransformTextMethodName = "TransformText";

		[NotNull]
		protected T4CSharpCodeGenerationResult Result { get; }

		[NotNull]
		protected IT4File File { get; }

		protected T4CSharpIntermediateConverterBase([NotNull] IT4File file)
		{
			File = file;
			Result = new T4CSharpCodeGenerationResult(File);
		}

		[NotNull]
		public T4CSharpCodeGenerationResult Convert(
			[NotNull] T4CSharpCodeGenerationIntermediateResult intermediateResult
		)
		{
			AppendGeneratedMessage();
			string ns = File
				.LogicalPsiSourceFile
				.ToProjectFile()
				.NotNull()
				.CalculateExpectedNamespace(T4Language.Instance);
			bool hasNamespace = !string.IsNullOrEmpty(ns);
			if (hasNamespace)
			{
				Result.AppendLine($"namespace {ns}");
				Result.AppendLine("{");
				PushIndent();
				AppendNamespaceContents(intermediateResult);
				PopIndent();
				Result.AppendLine("}");
			}
			else
			{
				AppendNamespaceContents(intermediateResult);
			}

			return Result;
		}

		private void AppendNamespaceContents([NotNull] T4CSharpCodeGenerationIntermediateResult intermediateResult)
		{
			AppendImports(intermediateResult);
			AppendClasses(intermediateResult);
		}

		protected virtual void AppendClasses([NotNull] T4CSharpCodeGenerationIntermediateResult intermediateResult)
		{
			AppendClass(intermediateResult);
			AppendIndent();
			Result.AppendLine();
			AppendBaseClass(intermediateResult);
		}

		private void AppendImports([NotNull] T4CSharpCodeGenerationIntermediateResult intermediateResult)
		{
			if (ShouldAppendPragmaDirectives)
			{
				AppendIndent();
				Result.AppendLine("#pragma warning disable 8019");
			}

			AppendIndent();
			Result.AppendLine("using System;");
			if (intermediateResult.HasHost)
			{
				AppendIndent();
				Result.AppendLine("using System.CodeDom.Compiler;");
			}

			if (ShouldAppendPragmaDirectives)
			{
				AppendIndent();
				Result.AppendLine("#pragma warning restore 8019");
			}

			foreach (var description in intermediateResult.ImportDescriptions)
			{
				description.AppendContent(Result, this);
			}
		}

		protected virtual void AppendClass([NotNull] T4CSharpCodeGenerationIntermediateResult intermediateResult)
		{
			AppendIndent();
			Result.Append($"public partial class {GeneratedClassName} : ");
			AppendBaseClassName(intermediateResult);
			Result.AppendLine();
			AppendIndent();
			Result.AppendLine("{");
			PushIndent();
			AppendProperties(intermediateResult);
			AppendConstructor(intermediateResult);
			AppendTransformMethod(intermediateResult);
			AppendParameterDeclarations(intermediateResult.ParameterDescriptions);
			AppendTemplateInitialization(intermediateResult.ParameterDescriptions);
			PopIndent();
			AppendIndent();
			Result.AppendLine("}");
		}

		private void AppendProperties([NotNull] T4CSharpCodeGenerationIntermediateResult intermediateResult)
		{
			if (intermediateResult.HasHost) AppendHost();
			foreach (var description in intermediateResult.FeatureDescriptions)
			{
				description.AppendContent(Result, this);
			}
		}

		protected virtual void AppendConstructor([NotNull] T4CSharpCodeGenerationIntermediateResult intermediateResult)
		{
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

		protected virtual void AppendFieldDeclaration([NotNull] T4ParameterDescription description)
		{
			AppendIndent();
			Result.Append("private global::");
			Result.Append(description.TypeString);
			Result.Append(" ");
			Result.Append(description.FieldNameString);
			Result.AppendLine(";");
		}

		protected virtual void AppendTemplateInitialization(
			[NotNull, ItemNotNull] IReadOnlyCollection<T4ParameterDescription> descriptions
		)
		{
			if (descriptions.IsEmpty()) return;
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

		protected virtual void AppendTransformMethod(
			[NotNull] T4CSharpCodeGenerationIntermediateResult intermediateResult
		)
		{
			AppendIndent();
			Result.Append("public ");
			Result.Append(intermediateResult.HasBaseClass ? "override" : "virtual");
			Result.AppendLine($" string {TransformTextMethodName}()");
			AppendIndent();
			Result.AppendLine("{");
			PushIndent();
			AppendIndent();
			Result.AppendLine();
			foreach (var description in intermediateResult.TransformationDescriptions)
			{
				description.AppendContent(Result, this);
			}

			AppendIndent();
			Result.AppendLine("return this.GenerationEnvironment.ToString();");
			PopIndent();
			AppendIndent();
			Result.AppendLine("}");
		}

		private void AppendBaseClassName([NotNull] T4CSharpCodeGenerationIntermediateResult intermediateResult)
		{
			if (intermediateResult.HasBaseClass) Result.Append(intermediateResult.CollectedBaseClass);
			else Result.Append(GeneratedBaseClassFQN);
		}

		private void AppendBaseClass([NotNull] T4CSharpCodeGenerationIntermediateResult intermediateResult)
		{
			if (intermediateResult.HasBaseClass) return;
			string resource = BaseClassResourceName;
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
		protected abstract string GeneratedBaseClassName { get; }

		[NotNull]
		protected abstract string GeneratedBaseClassFQN { get; }

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
		public abstract string ExpressionCommentStart { get; }
		public abstract string ExpressionCommentEnd { get; }
		public abstract string Indent { get; }
		public abstract bool ShouldBreakExpressionWithLineDirective { get; }
		public abstract void AppendCompilationOffset(T4CSharpCodeGenerationResult destination, IT4TreeNode node);
		public abstract void AppendLineDirective(T4CSharpCodeGenerationResult destination, IT4TreeNode node);
		public abstract void AppendMappedIfNeeded(T4CSharpCodeGenerationResult destination, IT4Code code);
		#endregion IT4ElementAppendFormatProvider
	}
}
