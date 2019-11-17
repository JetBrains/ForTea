using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.VisualStudio.TextTemplating
{
	[CLSCompliant(true)]
	public interface ITextTemplatingEngineHost
	{
		bool LoadIncludeText(string requestFileName, out string content, out string location);
		string ResolveAssemblyReference(string assemblyReference);
		IList<string> StandardAssemblyReferences { get; }
		IList<string> StandardImports { get; }
		Type ResolveDirectiveProcessor(string processorName);
		string ResolvePath(string path);
		string ResolveParameterValue(string directiveId, string processorName, string parameterName);
		AppDomain ProvideTemplatingAppDomain(string content);
		void LogErrors(CompilerErrorCollection errors);
		void SetFileExtension(string extension);
		void SetOutputEncoding(Encoding encoding, bool fromOutputDirective);
		string TemplateFile { get; }
		object GetHostOption(string optionName);
	}
}
