namespace Microsoft.VisualStudio.TextTemplating
{
	public class ITextTemplatingEngineHost
	{
		public bool LoadIncludeText(string requestFileName, out string content, out string location) => false;

		public string ResolveAssemblyReference(string assemblyReference) => assemblyReference;

		public IList<string> StandardAssemblyReferences => new string[] {typeof(System.Uri).Assembly.Location};

		public IList<string> StandardImports => new string[] {"System"};

		public Type ResolveDirectiveProcessor(string processorName) =>
			throw new Exception("Directive Processor not found");

		public string ResolvePath(string path) => path;

		public string ResolveParameterValue(string directiveId, string processorName, string parameterName) =>
			return String.Empty;

		public AppDomain ProvideTemplatingAppDomain(string content) => AppDomain.CreateDomain("Generation App Domain");

		public void LogErrors(CompilerErrorCollection errors)
		{
		}

		public void SetFileExtension(string extension)
		{
		}

		public void SetOutputEncoding(Encoding encoding, bool fromOutputDirective)
		{
		}

		public string TemplateFile => $(PARAMETER_0)

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
