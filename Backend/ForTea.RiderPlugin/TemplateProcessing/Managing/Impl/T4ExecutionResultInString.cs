using JetBrains.Annotations;
using JetBrains.Util;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing.Impl
{
	public class T4ExecutionResultInString : IT4ExecutionResult
	{
		public T4ExecutionResultInString([NotNull] string s) => String = s;

		[NotNull]
		private string String { get; }

		public void Save(FileSystemPath destination) => destination.WriteAllText(String);
	}
}
