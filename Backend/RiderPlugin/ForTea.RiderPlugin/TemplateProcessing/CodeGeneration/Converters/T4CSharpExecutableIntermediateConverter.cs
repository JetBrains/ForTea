using GammaJul.ForTea.Core.Parsing.Ranges;
using GammaJul.ForTea.Core.TemplateProcessing;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters.ClassName;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters.GeneratorKind;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ForTea.RiderPlugin.Psi.Resolve.Macros;
using JetBrains.ForTea.RiderPlugin.TemplateProcessing.CodeGeneration.Reference;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.Util;
using JetBrains.Util.dataStructures.TypedIntrinsics;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.CodeGeneration.Converters
{
  public sealed class T4CSharpExecutableIntermediateConverter : T4CSharpRealIntermediateConverter
  {
    [NotNull] private const string SuffixResource =
      "GammaJul.ForTea.Core.Resources.TemplateBaseFullExecutableSuffix.cs";

    [NotNull] private const string HostspecificSuffixResource =
      "GammaJul.ForTea.Core.Resources.HostspecificTemplateBaseFullExecutableSuffix.cs";

    [NotNull] private const string AssemblyRegisteringResource =
      "GammaJul.ForTea.Core.Resources.AssemblyRegistering.cs";

    [NotNull] private IT4ReferenceExtractionManager ReferenceExtractionManager { get; }

    public T4CSharpExecutableIntermediateConverter(
      [NotNull] IT4File file,
      [NotNull] IT4ReferenceExtractionManager referenceExtractionManager
    ) : base(file, new T4ExecutableClassNameProvider(), new T4ExecutableGeneratorKind())
    {
      file.AssertContainsNoIncludeContext();
      ReferenceExtractionManager = referenceExtractionManager;
    }

    // When creating executable, we reference JetBrains.TextTemplating,
    // which already contains the definition for TextTransformation
    protected override void AppendClasses(T4CSharpCodeGenerationIntermediateResult intermediateResult)
    {
      AppendMainContainer(intermediateResult);
      AppendClass(intermediateResult);
    }

    protected override void AppendHost()
    {
      AppendIndent();
      Result.AppendLine($"public virtual {T4TextTemplatingFQNs.HostInterface} Host {{ get; }}");
    }

    protected override bool ShouldAppendPragmaDirectives => true;

    protected override void AppendConstructor(T4CSharpCodeGenerationIntermediateResult intermediateResult)
    {
      if (!intermediateResult.HasHost) return;
      AppendIndent();
      Result.Append($"public GeneratedTextTransformation({T4TextTemplatingFQNs.Lifetime} lifetime)");
      AppendIndent();
      Result.AppendLine("{");
      PushIndent();
      AppendHostInitialization();
      PopIndent();
      AppendIndent();
      Result.AppendLine("}");
    }

    private void AppendHostInitialization()
    {
      AppendIndent();
      Result.AppendLine($"Host = new {T4TextTemplatingFQNs.HostImpl}(");
      PushIndent();
      {
        AppendIndent();
        Result.AppendLine("lifetime,");
        AppendIndent();
        Result.AppendLine($"new {T4TextTemplatingFQNs.Macros}");
        AppendIndent();
        Result.AppendLine("{");
        Result.AppendLine(GenerateExpandableMacros());
        AppendIndent();
        Result.AppendLine("},");
        AppendIndent();
        Result.Append("\"");
        string path = File.PhysicalPsiSourceFile.GetLocation().FullPath;
        Result.Append(StringLiteralConverter.EscapeToRegular(path));
        Result.AppendLine("\",");
        AppendIndent();
        Result.AppendLine("this);");
      }
      PopIndent();
    }

    [NotNull]
    private string GenerateExpandableMacros()
    {
      var projectFile = File.PhysicalPsiSourceFile.ToProjectFile();
      if (projectFile == null) return "";
      var resolver = File.GetSolution().GetComponent<IT4LightMacroResolver>();
      var macros = resolver.ResolveAllLightMacros(projectFile);
      return macros.AggregateString(",\n", (builder, pair) => builder
        .Append("{\"")
        .Append(StringLiteralConverter.EscapeToRegular(pair.Key))
        .Append("\", \"")
        .Append(StringLiteralConverter.EscapeToRegular(pair.Value))
        .Append("\"}")
      );
    }

    private void AppendMainContainer([NotNull] T4CSharpCodeGenerationIntermediateResult intermediateResult)
    {
      string resource = intermediateResult.HasHost ? HostspecificSuffixResource : SuffixResource;
      var provider = new T4TemplateResourceProvider(resource);
      string encoding = intermediateResult.Encoding ?? T4EncodingsManager.GetEncoding(File);
      string suffix = provider.ProcessResource(ClassNameProvider.GeneratedClassName, encoding);
      Result.Append(suffix);
      AppendAssemblyRegistering();
      // assembly registration code is part of main class,
      // so resources do not include closing brace
      Result.Append("}");
    }

    private void AppendAssemblyRegistering()
    {
      var provider = new T4TemplateResourceProvider(AssemblyRegisteringResource);
      string references = GetReferences();
      string registering = provider.ProcessResource(references);
      Result.Append(registering);
    }

    [NotNull]
    private string GetReferences() => ReferenceExtractionManager
      .ExtractReferenceLocationsTransitive(File)
      .AggregateString(",\n", (builder, it) => builder
        .Append("{\"")
        .Append(StringLiteralConverter.EscapeToRegular(it.FullName))
        .Append("\", \"")
        .Append(StringLiteralConverter.EscapeToRegular(it.Location.FullPath))
        .Append("\"}"));

    protected override string GetTransformTextOverridabilityModifier(bool hasCustomBaseClass) => OverrideKeyword;

    #region IT4ElementAppendFormatProvider

    public override string ToStringConversionPrefix =>
      T4TextTemplatingFQNs.ToStringHelper + ".ToStringWithCulture(";

    public override bool ShouldBreakExpressionWithLineDirective => true;

    public override void AppendMappedIfNeeded(T4CSharpCodeGenerationResult destination, IT4Code code) =>
      destination.Append(code.GetText());

    public override void AppendCompilationOffset(T4CSharpCodeGenerationResult destination, IT4TreeNode node)
    {
      int documentOffset = T4UnsafeManualRangeTranslationUtil.GetDocumentStartOffset(node).Offset;
      var lineOffset = node
        .FindLogicalPsiSourceFile()
        .Document
        .GetCoordsByOffset(documentOffset)
        .Column;
      for (var i = Int32<DocColumn>.O; i < lineOffset; i++)
      {
        destination.Append(" ");
      }
    }

    #endregion IT4ElementAppendFormatProvider
  }
}