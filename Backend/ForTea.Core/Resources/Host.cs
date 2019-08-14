namespace Microsoft.VisualStudio.TextTemplating
{
    public class ITextTemplatingEngineHost
    {
        private $(PARAMETER_1) transformation;
        
        public global::System.Text.Encoding Encoding { get; private set; }
        public string FileExtension { get; private set; }

        public bool LoadIncludeText(string requestFileName, out string content, out string location)
        {
            content = null;
            location = null;
            return false;
        }

        public string ResolveAssemblyReference(string assemblyReference)
        {
            assemblyReference = MacroResolveHelper.ExpandMacrosAndVariables(assemblyReference);
            if (System.IO.File.Exists(assemblyReference)) return assemblyReference;
            // TODO: search GAC
            return assemblyReference;
        }

        public System.Collections.Generic.IList<string> StandardAssemblyReferences =>
            new[] {typeof(System.Uri).Assembly.Location};

        public System.Collections.Generic.IList<string> StandardImports => new[] {"System"};

        public global::System.Type ResolveDirectiveProcessor(string processorName) =>
            throw new global::System.Exception("Directive Processor not found");

        public string ResolvePath(string path)
        {
            if (global::System.String.IsNullOrEmpty(path)) return path;
            if (global::System.IO.File.Exists(path)) return path;
            string directoryName = global::System.IO.Path.GetDirectoryName(this.TemplateFile);
            string str = global::System.IO.Path.Combine(directoryName, path);
            if (global::System.IO.File.Exists(str)) return str;
            if (global::System.IO.Directory.Exists(str)) return str;
            throw new global::System.IO.FileNotFoundException();
        }

        public string ResolveParameterValue(string directiveId, string processorName, string parameterName) =>
            String.Empty;

        public AppDomain ProvideTemplatingAppDomain(string content) => AppDomain.CreateDomain("Generation App Domain");

        public void LogErrors(CompilerErrorCollection errors) =>
            transformation.Errors.AddRange(errors);

        public void SetFileExtension(string extension) => FileExtension = extension;

        public void SetOutputEncoding(Encoding encoding, bool fromOutputDirective) => Encoding = encoding;

        public string TemplateFile => @"$(PARAMETER_0)";

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
    }

    public static class MacroResolveHelper
    {
        private static global::System.Text.RegularExpressions.Regex MacroRegex { get; } =
            new global::System.Text.RegularExpressions.Regex(@"\$\((\w+)\)",
                global::System.Text.RegularExpressions.RegexOptions.Compiled |
                global::System.Text.RegularExpressions.RegexOptions.IgnorePatternWhitespace);

        private global::System.Collections.Generic.IDictionary<string, string> KnownMacros { get; } =
            new global::System.Collections.Generic.Dictionary<string, string>
            {
                $(PARAMETER_2)
            };
        
        public string ExpandMacrosAndVariables(string s)
        {
            s = global::System.Environment.ExpandEnvironmentVariables(path)
            if (string.IsNullOrEmpty(s)) return s;
            var result = global::System.Environment.ExpandEnvironmentVariables(RawPath);
            return MacroRegex.Replace(result.ToString(), match =>
            {
                var group = match.Groups[1];
                string macro = group.Value;
                if (!group.Success) return macro;
                if (!KnownMacros.TryGetValue(macro, out string value)) return macro;
                return value;
            });
        }
    }
}
