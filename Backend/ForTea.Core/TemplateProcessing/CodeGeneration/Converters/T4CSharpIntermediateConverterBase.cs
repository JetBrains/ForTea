using System;
using System.Collections.Generic;
using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Descriptions;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters.ClassName;
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
		[NotNull] internal const string DefaultTransformTextMethodName = "TransformText";
		[NotNull] protected const string OverrideKeyword = "override";
		[NotNull] protected const string VirtualKeyword = "virtual";

		[NotNull]
		protected T4CSharpCodeGenerationResult Result { get; }

		[NotNull]
		protected IT4File File { get; }

		protected T4CSharpIntermediateConverterBase(
			[NotNull] IT4File file,
			[NotNull] IT4GeneratedClassNameProvider classNameProvider
		)
		{
			File = file;
			ClassNameProvider = classNameProvider;
			Result = new T4CSharpCodeGenerationResult(File);
		}

		[NotNull]
		public virtual T4CSharpCodeGenerationResult Convert(
			[NotNull] T4CSharpCodeGenerationIntermediateResult intermediateResult
		)
		{
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
			foreach (var description in intermediateResult.ImportDescriptions)
			{
				description.AppendContent(Result, this);
			}

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
		}

		protected virtual void AppendClass([NotNull] T4CSharpCodeGenerationIntermediateResult intermediateResult)
		{
			AppendIndent();
			Result.Append(intermediateResult.AccessRightsText);
			Result.Append($" partial class {ClassNameProvider.GeneratedClassName} : ");
			AppendBaseClassName(intermediateResult);
			Result.AppendLine();
			AppendIndent();
			Result.AppendLine("{");
			using (new IndentCookie(this))
			{
				AppendConstructor(intermediateResult);
				AppendTransformMethod(intermediateResult);
				AppendFeatures(intermediateResult);
				if (!intermediateResult.ParameterDescriptions.IsEmpty())
				{
					AppendIndent();
					Result.AppendLine();
					AppendIndent();
					Result.AppendLine($"#line 1 \"{File.LogicalPsiSourceFile.GetLocation()}\"");
					Result.AppendLine();
				}

				using (new UnindentCookie(this))
				{
					AppendParameterDeclarations(intermediateResult.ParameterDescriptions);
					AppendTemplateInitialization(intermediateResult.ParameterDescriptions);
				}

				if (!intermediateResult.ParameterDescriptions.IsEmpty())
				{
					Result.AppendLine();
					Result.AppendLine();
					AppendIndent();
					Result.AppendLine();
					AppendIndent();
					Result.AppendLine("#line default");
					AppendIndent();
					Result.AppendLine("#line hidden");
				}
			}
			AppendIndent();
			Result.AppendLine("}");
		}

		private void AppendFeatures([NotNull] T4CSharpCodeGenerationIntermediateResult intermediateResult)
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
			Result.Append("private ");
			description.AppendType(Result);
			Result.Append(" ");
			Result.Append(description.FieldNameString);
			Result.AppendLine(";");
			Result.AppendLine();
		}

		protected virtual void AppendTemplateInitialization(
			[NotNull, ItemNotNull] IReadOnlyCollection<T4ParameterDescription> descriptions
		)
		{
			using var unindent = new UnindentCookie(this);
			if (descriptions.IsEmpty()) return;
			Result.AppendLine(@"
/// <summary>
/// Initialize the template
/// </summary>
public virtual void Initialize()");
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
			if (!TransformTextAttributes.IsNullOrEmpty())
			{
				AppendIndent();
				Result.AppendLine(TransformTextAttributes);
			}
			AppendIndent();
			Result.Append("public ");
			Result.Append(GetTransformTextOverridabilityModifier(intermediateResult.HasBaseClass));
			Result.AppendLine($" string {TransformTextMethodName}()");
			AppendIndent();
			Result.AppendLine("{");
			using (new IndentCookie(this))
			{
				foreach (var description in intermediateResult.TransformationDescriptions)
				{
					description.AppendContent(Result, this);
				}

				AppendIndent();
				Result.AppendLine("return this.GenerationEnvironment.ToString();");
			}
			AppendIndent();
			Result.AppendLine("}");
		}

		private void AppendBaseClassName([NotNull] T4CSharpCodeGenerationIntermediateResult intermediateResult)
		{
			if (intermediateResult.HasBaseClass) Result.Append(intermediateResult.CollectedBaseClass);
			else Result.Append(ClassNameProvider.GeneratedBaseClassFQN);
		}

		protected virtual void AppendBaseClass([NotNull] T4CSharpCodeGenerationIntermediateResult intermediateResult)
		{
			if (intermediateResult.HasBaseClass) return;
			Result.AppendLine(BaseClassDescription);
			AppendIndent();
			Result.Append(intermediateResult.AccessRightsText);
			Result.Append(" class ");
			Result.AppendLine(ClassNameProvider.GeneratedBaseClassName);
			string resource = BaseClassResourceName;
			var provider = new T4TemplateResourceProvider(resource);
			Result.Append(provider.Template);
		}

		[NotNull]
		protected virtual string TransformTextMethodName => DefaultTransformTextMethodName;

		[NotNull]
		protected virtual string TransformTextAttributes => "";

		protected abstract void AppendHost();
		protected abstract void AppendParameterDeclaration([NotNull] T4ParameterDescription description);

		[NotNull]
		protected abstract string BaseClassDescription { get; }

		[NotNull]
		protected abstract string BaseClassResourceName { get; }

		[NotNull]
		protected IT4GeneratedClassNameProvider ClassNameProvider { get; }

		protected abstract void AppendParameterInitialization(
			[NotNull, ItemNotNull] IReadOnlyCollection<T4ParameterDescription> descriptions);

		protected virtual bool ShouldAppendPragmaDirectives => false;

		[NotNull]
		protected virtual string GetTransformTextOverridabilityModifier(bool hasCustomBaseClass) => OverrideKeyword;

		#region Indentation
		protected int CurrentIndent { get; private set; }
		protected void PushIndent() => CurrentIndent += 1;
		protected void PopIndent() => CurrentIndent -= 1;
		protected void AppendIndent() => AppendIndent(CurrentIndent);
		protected abstract void AppendIndent(int size);

		protected sealed class IndentCookie : IDisposable
		{
			[NotNull]
			private T4CSharpIntermediateConverterBase Converter { get; }

			public IndentCookie([NotNull] T4CSharpIntermediateConverterBase converter)
			{
				Converter = converter;
				Converter.PushIndent();
			}

			public void Dispose() => Converter.PopIndent();
		}

		protected sealed class UnindentCookie : IDisposable
		{
			[NotNull]
			private T4CSharpIntermediateConverterBase Converter { get; }

			private int SavedIndent { get; }

			public UnindentCookie([NotNull] T4CSharpIntermediateConverterBase converter)
			{
				Converter = converter;
				SavedIndent = Converter.CurrentIndent;
				Converter.CurrentIndent = 0;
			}

			public void Dispose()
			{
				Converter.CurrentIndent = SavedIndent;
			}
		}

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
