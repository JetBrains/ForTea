namespace Microsoft.VisualStudio.TextTemplating
{
	[global::System.CLSCompliant(true)]
	public interface ITextTemplatingEngineHost
	{
		bool LoadIncludeText(string requestFileName, out string content, out string location);

		string ResolveAssemblyReference(string assemblyReference);

        global::System.Collections.Generic.IList<string> StandardAssemblyReferences { get; }

        global::System.Collections.Generic.IList<string> StandardImports { get; }

        global::System.Type ResolveDirectiveProcessor(string processorName);

		string ResolvePath(string path);

		string ResolveParameterValue(string directiveId, string processorName, string parameterName);

        global::System.AppDomain ProvideTemplatingAppDomain(string content);

		void LogErrors(CompilerErrorCollection errors);

		void SetFileExtension(string extension);

		void SetOutputEncoding(Encoding encoding, bool fromOutputDirective);

		string TemplateFile { get; }

		object GetHostOption(string optionName);
	}
}
