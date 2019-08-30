using System;
using System.Collections.Generic;
using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Descriptions;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;
using JetBrains.Util.dataStructures.TypedIntrinsics;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters
{
	public abstract class T4CSharpIntermediateConverterBase : IT4ElementAppendFormatProvider
	{
		[NotNull] public const string GeneratedClassNameString = "TextTransformation";
		[NotNull] public const string GeneratedBaseClassNameString = GeneratedClassNameString + "Base";
		[NotNull] internal const string TransformTextMethodName = "TransformText";

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
			bool hasNamespace = !String.IsNullOrEmpty(ns);
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

		private void AppendNamespaceContents()
		{
			AppendImports();
			AppendClasses(IntermediateResult.HasHost);
		}

		protected virtual void AppendClasses(bool hostspecific)
		{
			AppendClass();
			AppendIndent();
			Result.AppendLine();
			AppendBaseClass();
		}

		[CanBeNull]
		private string GetNamespace()
		{
			var sourceFile = File.GetSourceFile();
			var projectFile = sourceFile?.ToProjectFile();

			string ns = projectFile.GetCustomToolNamespace();
			string ns2 = sourceFile?.Properties.GetDefaultNamespace();
			if (ns == null) return ns2;
			if (ns2 == null) return ns;
			return ns.IsEmpty() ? ns2 : ns;
		}

		protected virtual void AppendImports()
		{
			foreach (var description in IntermediateResult.ImportDescriptions)
			{
				AppendIndent();
				Result.Append("using ");
				if (description.HasSameSource(File))
					Result.AppendMapped(description.Presentation, description.Source.GetTreeTextRange());
				else Result.Append(description.Presentation);
				Result.AppendLine(";");
			}

			AppendIndent();
			Result.AppendLine("using System;");
			if (!IntermediateResult.HasHost) return;
			AppendIndent();
			Result.AppendLine("using System.CodeDom.Compiler;");
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
				description.AppendContent(Result, this, File);
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
			AppendTransformationPrefix();
			foreach (var description in IntermediateResult.TransformationDescriptions)
			{
				description.AppendContent(Result, this, File);
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
			if (IntermediateResult.HasBaseClass) return;
			var provider = new T4TemplateResourceProvider(ResourceName, this);
			Result.Append(provider.ProcessResource(GeneratedBaseClassName));
		}

		protected abstract void AppendHost();
		protected abstract void AppendParameterDeclaration([NotNull] T4ParameterDescription description);

		[NotNull]
		protected abstract string ResourceName { get; }

		[NotNull]
		protected virtual string GeneratedClassName => GeneratedClassNameString;

		[NotNull]
		protected virtual string GeneratedBaseClassName => GeneratedBaseClassNameString;

		protected abstract void AppendSyntheticAttribute();

		protected abstract void AppendParameterInitialization(
			[NotNull, ItemNotNull] IReadOnlyCollection<T4ParameterDescription> descriptions);

		protected virtual void AppendTransformationPrefix()
		{
		}

		protected virtual void AppendGeneratedMessage()
		{
		}

		#region Indentation
		protected int CurrentIndent { get; private set; }
		protected void PushIndent() => CurrentIndent += 1;
		protected void PopIndent() => CurrentIndent -= 1;
		protected void AppendIndent() => AppendIndent(CurrentIndent);
		protected abstract void AppendIndent(int size);
		#endregion Indentation

		#region IT4ElementAppendFormatProvider
		public abstract string ToStringConversionPrefix { get; }
		public abstract string ToStringConversionSuffix { get; }
		public abstract string ExpressionWritingPrefix { get; }
		public abstract string ExpressionWritingSuffix { get; }
		public abstract string CodeCommentStart { get; }
		public abstract string CodeCommentEnd { get; }
		public abstract string Indent { get; }
		public abstract bool ShouldBreakExpressionWithLineDirective { get; }
		public abstract void AppendCompilationOffset(T4CSharpCodeGenerationResult destination, Int32<DocColumn> offset);
		public abstract void AppendMappedIfNeeded(T4CSharpCodeGenerationResult destination, IT4Code code);
		#endregion IT4ElementAppendFormatProvider
	}
}
