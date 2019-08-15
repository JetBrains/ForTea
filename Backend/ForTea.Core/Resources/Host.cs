namespace Microsoft.VisualStudio.TextTemplating
{
    public class ITextTemplatingEngineHost
    {
        private $(PARAMETER_1) transformation;
        
        public global::System.Text.Encoding Encoding { get; private set; }
        public string FileExtension { get; private set; }

        public bool LoadIncludeText(string requestFileName, out string content, out string location)
        {
            if (string.IsNullOrEmpty(requestFileName))
            {
                content = string.Empty;
                location = string.Empty;
                return false;
            }

            requestFileName = MacroResolveHelper.ExpandMacrosAndVariables(requestFileName);
            if (global::System.IO.Path.IsPathRooted(requestFileName))
            {
                location = requestFileName;
                content = this.LoadContent(requestFileName);
                return true;
            }

            string folderPath = global::System.IO.Path.GetDirectoryName(this.TemplateFile);
            string candidate = global::System.IO.Path.Combine(folderPath, requestFileName);
            if (System.IO.Path.IsPathRooted(candidate))
            {
                location = candidate;
                content = this.LoadContent(candidate);
                return true;
            }

            location = string.Empty;
            content = string.Empty;
            return false;
        }

        private string LoadContent(string path)
        {
            using (global::System.IO.StreamReader streamReader = new StreamReader(path))
            {
                return streamReader.ReadToEnd();
            }
        }

        public string ResolveAssemblyReference(string assemblyReference)
        {
            assemblyReference = MacroResolveHelper.ExpandMacrosAndVariables(assemblyReference);
            if (global::System.IO.Path.IsPathRooted(assemblyReference)) return assemblyReference;
            // TODO: search GAC
            return assemblyReference;
        }

        public global::System.Collections.Generic.IList<string> StandardAssemblyReferences =>
            new[] {typeof(global::System.Uri).Assembly.Location};

        public global::System.Collections.Generic.IList<string> StandardImports => new[] {"System"};

        public global::System.Type ResolveDirectiveProcessor(string processorName) =>
            throw new global::System.Exception("Directive Processor not found");

        public string ResolvePath(string path)
        {
            if (global::System.String.IsNullOrEmpty(path)) return path;
            if (global::System.IO.Path.IsPathRooted(path)) return path;
            string directoryName = global::System.IO.Path.GetDirectoryName(this.TemplateFile);
            string candidate = global::System.IO.Path.Combine(directoryName, path);
            if (global::System.IO.Path.IsPathRooted(candidate)) return candidate;
            throw new global::System.IO.FileNotFoundException();
        }

        public string ResolveParameterValue(string directiveId, string processorName, string parameterName) =>
            string.Empty;

        public global::System.AppDomain ProvideTemplatingAppDomain(string content) => AppDomain.CreateDomain("Generation App Domain");

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

        private static global::System.Collections.Generic.IDictionary<string, string> KnownMacros { get; } =
            new global::System.Collections.Generic.Dictionary<string, string>
            {
$(PARAMETER_2)
            };
        
        public static string ExpandMacrosAndVariables(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            return MacroRegex.Replace(global::System.Environment.ExpandEnvironmentVariables(s), match =>
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
