using System;
using JetBrains.Annotations;
using JetBrains.Util;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing
{
	[Obsolete("Template execution has moved to frontend")]
	public interface IT4ExecutionResult
	{
		void Save([NotNull] FileSystemPath destination);
	}
}
