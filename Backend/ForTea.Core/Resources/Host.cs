namespace Microsoft.VisualStudio.TextTemplating
{
    public class ITextTemplatingEngineHost
    {
        private $(PARAMETER_1) transformation;
        
        public Encoding Encoding { get; private set; }
        public string FileExtension { get; private set; }

        public bool LoadIncludeText(string requestFileName, out string content, out string location)
        {
            content = null;
            location = null;
            return false;
        }

        public string ResolveAssemblyReference(string assemblyReference) => assemblyReference;

        public System.Collections.Generic.IList<string> StandardAssemblyReferences =>
            new[] {typeof(System.Uri).Assembly.Location};

        public System.Collections.Generic.IList<string> StandardImports => new[] {"System"};

        public Type ResolveDirectiveProcessor(string processorName) =>
            throw new Exception("Directive Processor not found");

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
}
