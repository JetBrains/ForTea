using System;
using System.Collections.Generic;
using GammaJul.ForTea.Core.Parsing.Ranges;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Descriptions;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters.ClassName;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters.GeneratorKind;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.CodeGeneration.Converters
{
  public class T4CSharpRealIntermediateConverter : T4CSharpIntermediateConverterBase
  {
    const int indentSize = 4;

    public T4CSharpRealIntermediateConverter(
      [NotNull] IT4File file,
      [NotNull] IT4GeneratedClassNameProvider classNameProvider
    ) : this(file, classNameProvider, new T4PreprocessedGeneratorKind())
    {
    }

    protected T4CSharpRealIntermediateConverter(
      [NotNull] IT4File file,
      [NotNull] IT4GeneratedClassNameProvider classNameProvider,
      [NotNull] IT4GeneratorKind generatorKind
    ) : base(file, classNameProvider, generatorKind)
    {
    }

    protected sealed override string BaseClassResourceName => "GammaJul.ForTea.Core.Resources.TemplateBaseFull.cs.template";

    protected sealed override void AppendParameterInitialization(
      IReadOnlyCollection<T4ParameterDescription> descriptions,
      bool hasHost
    )
    {
      AppendIndent();
      Result.AppendLine("if ((this.Errors.HasErrors == false))");
      AppendIndent();
      Result.AppendLine("{");
      using (new UnindentCookie(this))
      {
        foreach (var description in descriptions)
        {
          AppendPropertyInitializationFromSession(description);
          AppendPropertyInitializationFromHost(hasHost, description);
          AppendPropertyInitializationFromContext(description);
        }
      }

      Result.AppendLine();
      Result.AppendLine();
      AppendIndent();
      Result.AppendLine("}");
    }

    private void AppendPropertyInitializationFromSession([NotNull] T4ParameterDescription description)
    {
      AppendIndent();
      Result.AppendLine($"bool {description.PropertyNameString}ValueAcquired = false;");
      AppendIndent();
      Result.AppendLine($@"if (this.Session.ContainsKey(""{description.PropertyNameString}""))");
      using (new CodeBlockCookie(this))
      {
        AppendIndent();
        Result.Append($"this.{description.FieldNameString} = ((");
        description.AppendType(Result);
        Result.AppendLine($@")(this.Session[""{description.PropertyNameString}""]));");
        AppendIndent();
        Result.AppendLine($"{description.PropertyNameString}ValueAcquired = true;");
      }
    }

    private void AppendPropertyInitializationFromHost(bool hasHost, T4ParameterDescription description)
    {
      if (!hasHost) return;
      AppendIndent();
      Result.AppendLine($"if (({description.PropertyNameString}ValueAcquired == false))");
      using (new CodeBlockCookie(this))
      {
        AppendIndent();
        Result.AppendLine(
          "string parameterValue = this.Host.ResolveParameterValue(\"Property\"," +
          $" \"PropertyDirectiveProcessor\", \"{description.PropertyNameString}\");");
        AppendIndent();
        Result.AppendLine("if ((string.IsNullOrEmpty(parameterValue) == false))");
        using (new CodeBlockCookie(this))
        {
          AppendIndent();
          Result.Append(
            $"{T4TextTemplatingFQNs.TypeConverter} tc = {T4TextTemplatingFQNs.GetConverter}(typeof(");
          description.AppendType(Result);
          Result.AppendLine("));");
          AppendIndent();
          Result.AppendLine("if (((tc != null) ");
          AppendIndent();
          Result.Append(new string(' ', indentSize * 3));
          Result.AppendLine("&& tc.CanConvertFrom(typeof(string))))");
          using (new CodeBlockCookie(this))
          {
            AppendIndent();
            Result.Append($"this.{description.FieldNameString} = ((");
            description.AppendType(Result);
            Result.AppendLine(")(tc.ConvertFrom(parameterValue)));");
            AppendIndent();
            Result.AppendLine($"{description.PropertyNameString}ValueAcquired = true;");
          }

          AppendIndent();
          Result.AppendLine("else");
          using (new CodeBlockCookie(this))
          {
            AppendTypeErrorMessage(description);
          }
        }
      }
    }

    private void AppendTypeErrorMessage([NotNull] T4ParameterDescription description)
    {
      AppendIndent();
      Result.Append("this.Error(");
      string message =
        $"The type \\'{description.TypeFqnString}\\' of the parameter " +
        $"\\'{description.PropertyNameString}\\' did not match" +
        " the type of the data passed to the template.";
      const int messageChunkSize = 85;
      int index = 0;
      foreach (string chunk in SplitStringIntoChunks(message, messageChunkSize))
      {
        if (index != 0)
        {
          Result.AppendLine(" +");
          AppendIndent();
          Result.Append(new string(' ', indentSize * 2));
        }

        Result.Append($"\"{chunk}\"");
        index += 1;
      }

      Result.AppendLine(");");
    }

    private void AppendPropertyInitializationFromContext([NotNull] T4ParameterDescription description)
    {
      AppendIndent();
      Result.AppendLine($"if (({description.PropertyNameString}ValueAcquired == false))");
      using (new CodeBlockCookie(this))
      {
        AppendIndent();
        Result.AppendLine(
          $@"object data = global::System.Runtime.Remoting.Messaging.CallContext.LogicalGetData(""{description.PropertyNameString}"");");
        AppendIndent();
        Result.AppendLine("if ((data != null))");
        using (new CodeBlockCookie(this))
        {
          AppendIndent();
          Result.Append($"this.{description.FieldNameString} = ((");
          description.AppendType(Result);
          Result.AppendLine(")(data));");
        }
      }
    }

    [NotNull, ItemNotNull]
    private static IEnumerable<string> SplitStringIntoChunks([NotNull] string source, int chunkSize)
    {
      for (int index = 0; index <= source.Length / chunkSize; index += 1)
      {
        yield return source.Substring(index * chunkSize, Math.Min(chunkSize, source.Length - index * chunkSize));
      }
    }

    protected override void AppendClass(T4CSharpCodeGenerationIntermediateResult intermediateResult)
    {
      AppendIndent();
      Result.AppendLine();
      AppendClassSummary();
      if (intermediateResult.UseLinePragmas)
      {
        AppendIndent();
        Result.AppendLine();
        AppendIndent();
        File.AssertContainsNoIncludeContext();
        Result.AppendLine($"#line 1 \"{File.PhysicalPsiSourceFile.GetLocation()}\"");
      }

      AppendIndent();
      Result.AppendLine(GeneratedCodeAttribute);
      base.AppendClass(intermediateResult);
    }

    protected override void AppendTransformMethod(T4CSharpCodeGenerationIntermediateResult intermediateResult)
    {
      if (intermediateResult.UseLinePragmas) Result.AppendLine("#line hidden");
      AppendIndent();
      Result.AppendLine("/// <summary>");
      AppendIndent();
      Result.AppendLine("/// Create the template output");
      AppendIndent();
      Result.AppendLine("/// </summary>");
      base.AppendTransformMethod(intermediateResult);
    }

    private void AppendClassSummary()
    {
      AppendIndent();
      Result.AppendLine("/// <summary>");
      AppendIndent();
      Result.AppendLine("/// Class to produce the template output");
      AppendIndent();
      Result.AppendLine("/// </summary>");
    }

    public override T4CSharpCodeGenerationResult Convert(
      T4CSharpCodeGenerationIntermediateResult intermediateResult
    )
    {
      Result.Append($@"// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: {T4TextTemplatingFQNs.RuntimeVersion}
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
");
      return base.Convert(intermediateResult);
    }

    protected override void AppendParameterDeclaration(T4ParameterDescription description)
    {
      // Range maps of this converter are ignored, so it's safe to use Append instead of AppendMapped
      Result.AppendLine($@"/// <summary>
/// Access the {description.PropertyNameString} parameter of the template.
/// </summary>");
      AppendIndent();
      Result.Append("private ");
      description.AppendTypeMapped(Result);
      Result.Append(" ");
      description.AppendName(Result);
      Result.AppendLine();
      using (new CodeBlockCookie(this))
      {
        AppendIndent();
        Result.AppendLine("get");
        using (new CodeBlockCookie(this))
        {
          AppendIndent();
          Result.AppendLine($"return this.{description.FieldNameString};");
        }
      }

      Result.AppendLine();
    }

    protected override void AppendHost()
    {
      AppendIndent();
      Result.AppendLine($"private {T4TextTemplatingFQNs.HostInterface} hostValue;");
      AppendIndent();
      Result.AppendLine("/// <summary>");
      AppendIndent();
      Result.AppendLine("/// The current host for the text templating engine");
      AppendIndent();
      Result.AppendLine("/// </summary>");
      AppendIndent();
      Result.AppendLine($"public virtual {T4TextTemplatingFQNs.HostInterface} Host");
      using (new CodeBlockCookie(this))
      {
        AppendIndent();
        Result.AppendLine("get");
        using (new CodeBlockCookie(this))
        {
          AppendIndent();
          Result.AppendLine("return this.hostValue;");
        }

        AppendIndent();
        Result.AppendLine("set");
        using (new CodeBlockCookie(this))
        {
          AppendIndent();
          Result.AppendLine("this.hostValue = value;");
        }
      }
    }

    protected override void AppendIndent(int size)
    {
      // TODO: use user indents?
      for (int index = 0; index < size; index += 1)
      {
        Result.Append("    ");
      }
    }

    protected override string BaseClassDescription =>
      $@"    {(ShouldUseLineDirectives? @"#line default
    #line hidden
    " : "")}#region Base class
    /// <summary>
    /// Base class for this transformation
    /// </summary>
    {GeneratedCodeAttribute}";

    [NotNull]
    private static string GeneratedCodeAttribute { get; } =
      $@"[{T4TextTemplatingFQNs.GeneratedAttribute}(""{T4TextTemplatingFQNs.TextTemplating}"", ""{T4TextTemplatingFQNs.RuntimeVersion}"")]";

    protected override string GetTransformTextOverridabilityModifier(bool hasCustomBaseClass)
    {
      if (!hasCustomBaseClass) return VirtualKeyword;
      return base.GetTransformTextOverridabilityModifier(true);
    }

    #region IT4ElementAppendFormatProvider

    public override string CodeCommentStart => "";
    public override string CodeCommentEnd => "";
    public override string ExpressionCommentStart => "";
    public override string ExpressionCommentEnd => "";
    public override string Indent => new string(' ', CurrentIndent * 4); // TODO: use user indents?
    public override bool ShouldBreakExpressionWithLineDirective => false;

    public override void AppendCompilationOffset(T4CSharpCodeGenerationResult destination, IT4TreeNode node)
    {
      // In preprocessed file, behave like VS
    }

    public override void AppendLineDirective(T4CSharpCodeGenerationResult destination, IT4TreeNode node)
    {
      if (!ShouldUseLineDirectives) return;
      destination.Append(Indent);
      var sourceFile = node.FindLogicalPsiSourceFile();
      var offset = T4UnsafeManualRangeTranslationUtil.GetDocumentStartOffset(node);
      int line = (int)offset.ToDocumentCoords().Line;
      destination.AppendLine($"#line {line + 1} \"{sourceFile.GetLocation()}\"");
    }

    public override void AppendMappedIfNeeded(T4CSharpCodeGenerationResult destination, IT4Code code) =>
      destination.Append(code.GetText().Trim());

    #endregion IT4ElementAppendFormatProvider
  }
}