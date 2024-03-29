using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Text;
using EnvDTE;
using EnvDTE80;
using JetBrains.EnvDTE.Client;
using JetBrains.EnvDTE.Client.Impl;
using JetBrains.Lifetimes;
using JetBrains.Util;

namespace Microsoft.VisualStudio.TextTemplating.JetBrains
{
  public class TextTemplatingEngineHost : ITextTemplatingEngineHost, IServiceProvider
  {
    private const string RelativeUnsupportedMessage =
      "Relative include include path resolution is not supported yet.\n" +
      "Please, provide absolute path, use macros like $(SolutionDir) or\n" +
      "contact support at https://youtrack.jetbrains.com/newissue";

    public TextTransformation Transformation { get; }
    public Encoding Encoding { get; private set; }
    public string FileExtension { get; private set; }

    public TextTemplatingEngineHost(
      Lifetime lifetime,
      IDictionary<string, string> knownMacros,
      string templateFile,
      TextTransformation transformation
    )
    {
      Lifetime = lifetime;
      Helper = new MacroResolveHelper(knownMacros);
      Transformation = transformation;
      TemplateFile = templateFile;
    }

    private MacroResolveHelper Helper { get; }

    private Lifetime Lifetime { get; }

    private DteImplementation CachedDteImplementation { get; set; }

    public bool LoadIncludeText(string requestFileName, out string content, out string location)
    {
      if (string.IsNullOrEmpty(requestFileName))
      {
        content = string.Empty;
        location = string.Empty;
        return false;
      }

      requestFileName = Helper.ExpandMacrosAndVariables(requestFileName);
      if (Path.IsPathRooted(requestFileName))
      {
        location = requestFileName;
        content = LoadContent(requestFileName);
        return true;
      }

      string folderPath = Path.GetDirectoryName(TemplateFile);
      string candidate = Path.Combine(folderPath, requestFileName);
      if (Path.IsPathRooted(candidate))
      {
        location = candidate;
        content = LoadContent(candidate);
        return true;
      }

      Transformation.Warning(RelativeUnsupportedMessage);
      location = string.Empty;
      content = string.Empty;
      return false;
    }

    private string LoadContent(string path)
    {
      using (StreamReader streamReader = new StreamReader(path))
      {
        return streamReader.ReadToEnd();
      }
    }

    public string ResolveAssemblyReference(string assemblyReference)
    {
      assemblyReference = Helper.ExpandMacrosAndVariables(assemblyReference);
      if (Path.IsPathRooted(assemblyReference)) return assemblyReference;
      Transformation.Warning(RelativeUnsupportedMessage);
      return assemblyReference;
    }

    public IList<string> StandardAssemblyReferences =>
      new[] { typeof(Uri).Assembly.GetPath().FullPath };

    public IList<string> StandardImports => new[] { "System" };

    public Type ResolveDirectiveProcessor(string processorName) =>
      throw new Exception("Directive Processor not found");

    public string ResolvePath(string path)
    {
      if (path == null) throw new ArgumentNullException(nameof(path));
      if (path == string.Empty) return Path.GetDirectoryName(TemplateFile);
      if (Path.IsPathRooted(path)) return path;
      string directoryName = Path.GetDirectoryName(TemplateFile);
      string candidate = Path.Combine(directoryName, path);
      if (Path.IsPathRooted(candidate)) return candidate;
      throw new FileNotFoundException();
    }

    public string ResolveParameterValue(string directiveId, string processorName, string parameterName) =>
      string.Empty;

    public AppDomain ProvideTemplatingAppDomain(string content) =>
      AppDomain.CreateDomain("Generation App Domain");

    public void LogErrors(CompilerErrorCollection errors) =>
      Transformation.Errors.AddRange(errors);

    public void SetFileExtension(string extension) => FileExtension = extension;
    public void SetOutputEncoding(Encoding encoding, bool fromOutputDirective) => Encoding = encoding;
    public string TemplateFile { get; }

    public object GetHostOption(string optionName)
    {
      switch (optionName)
      {
        case "CacheAssemblies":
          return true;
        default:
          return null;
      }
    }

    public object GetService(Type serviceType)
    {
      if (serviceType == typeof(DTE) || serviceType == typeof(DTE2))
      {
        if (CachedDteImplementation != null) return CachedDteImplementation;
        string rawPort = Environment.GetEnvironmentVariable("T4_ENVDTE_CLIENT_PORT");
        if (rawPort == null) return null;
        if (!int.TryParse(rawPort, out int port)) return null;
        var manager = new ConnectionManager(Lifetime, port);
        var implementation = new DteImplementation(manager.Model);
        CachedDteImplementation = implementation;
        return implementation;
      }

      return null;
    }
  }
}