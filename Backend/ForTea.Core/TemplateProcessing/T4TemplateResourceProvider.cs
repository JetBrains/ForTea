using System.IO;
using System.Reflection;
using JetBrains.Annotations;
using JetBrains.Diagnostics;

namespace GammaJul.ForTea.Core.TemplateProcessing
{
	// TODO: make this shell component
	public class T4TemplateResourceProvider
	{
		[NotNull]
		private string Template { get; }

		[NotNull]
		private static string ReadTemplate([NotNull] string resourceName)
		{
			var assembly = Assembly.GetExecutingAssembly();
			using var stream = assembly.GetManifestResourceStream(resourceName);
			using var reader = new StreamReader(stream.NotNull());
			return reader.ReadToEnd();
		}

		[NotNull]
		public string ProcessResource(params string[] parameters)
		{
			string result = Template;
			for (int index = 0; index < parameters.Length; index += 1)
			{
				string parameter = parameters[index];
				result = result.Replace($"$(PARAMETER_{index})", parameter);
			}

			return result;
		}

		public T4TemplateResourceProvider([NotNull] string resourceName) => Template = ReadTemplate(resourceName);
	}
}
