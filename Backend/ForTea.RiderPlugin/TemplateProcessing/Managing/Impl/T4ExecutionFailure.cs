using JetBrains.Annotations;
using JetBrains.Util;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing.Impl
{
	public sealed class T4ExecutionFailure : T4ExecutionResultInString
	{
		[NotNull] private const string ErrorMessage = "ErrorGeneratingOutput";
		public T4ExecutionFailure() : base(ErrorMessage)
		{
		}
	}
}
