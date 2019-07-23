using System.Diagnostics;
using JetBrains.Annotations;
using JetBrains.Util;

namespace JetBrains.ForTea.RiderPlugin.TemplateProcessing.Managing.Impl
{
	public readonly struct T4ProcessInfo
	{
		[NotNull]
		public Process Process { get; }

		[NotNull]
		public FileSystemPath OutputFile { get; }

		public T4ProcessInfo(Process process, FileSystemPath outputFile)
		{
			Process = process;
			OutputFile = outputFile;
		}
	}
}
