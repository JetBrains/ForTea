using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GammaJul.ForTea.Core.Parsing.Lexing;
using GammaJul.ForTea.Core.Parsing.Parser;
using GammaJul.ForTea.Core.Test;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ForTea.Tests.Mock;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;
using NUnit.Framework;

namespace JetBrains.ForTea.Tests.Psi
{
	public sealed class T4CloningParserStressTest : T4CloningParserTestBase
	{
		private const int MEASUREMENT_COUNT = 1000;

		const string includeText =
			"<#@ using namespace=\"System.Text\" #>\n<#+\n    public void Foo()\n    {\n    }\n#>";

		// [Test]
		public void StressTest()
		{
			Console.WriteLine("Generating data...");
			string text = GenerateLargeString(1000);
			Console.WriteLine("Warming up...");
			WarmUpJit();
			GC.Collect();
			var parseTimes = new List<long>();
			var cloneTimes = new List<long>();
			Console.WriteLine("Measuring...");
			for (int i = 0; i < MEASUREMENT_COUNT; i += 1)
			{
				IFile file;
				using (new MyTimeMeasurementCookie(parseTimes))
				{
					file = T4ParserExposer.Create(text, new T4ParsingMockIncludeParser(includeText), new T4MockPsiSourceFile()).ParseFile();
					InitializeResolvePaths(((IT4File) file)!);
				}

				using (new MyTimeMeasurementCookie(cloneTimes))
				{
					new T4CloningParser(new T4MockPsiFileProvider(file!), null, T4DocumentLexerSelector.Instance)
						.ParseFile();
				}
			}

			Console.WriteLine("Done!");
			Console.WriteLine("{0} {1} {2}", "operation", "mean", "stddev");
			Console.WriteLine("{0} {1} {2}", "parse", parseTimes.Average(), StdDev(parseTimes));
			Console.WriteLine("{0} {1} {2}", "clone", cloneTimes.Average(), StdDev(cloneTimes));
		}

		private static double StdDev([NotNull] ICollection<long> data)
		{
			if (data.Count <= 1) return 0;
			double avg = data.Average();
			return Math.Sqrt(data.Sum(d => (d - avg) * (d - avg)) / data.Count);
		}

		private static void WarmUpJit()
		{
			string text = GenerateLargeString(100);
			for (int i = 0; i < 10; i += 1)
			{
				var dummyData = new List<long>();
				using var cookie = new MyTimeMeasurementCookie(dummyData);
				var file = T4ParserExposer.Create(text, new T4ParsingMockIncludeParser(includeText), new T4MockPsiSourceFile()).ParseFile();
				InitializeResolvePaths(((IT4File) file)!);
				new T4CloningParser(new T4MockPsiFileProvider(file!), null, T4DocumentLexerSelector.Instance).ParseFile();
			}
		}

		[NotNull]
		private static string GenerateLargeString(int numberOfRepetitions)
		{
			var lines = new List<string>();
			for (int i = 0; i < numberOfRepetitions; i += 1)
			{
				lines.Add("<#@ template language=\"C#\" #>\n<#@ include file=\"dummy.ttinclude\" #>");
			}

			return lines.AggregateString("\n");
		}

		private sealed class MyTimeMeasurementCookie : IDisposable
		{
			[NotNull]
			private Stopwatch Stopwatch { get; }

			[NotNull]
			private List<long> Data { get; }

			public MyTimeMeasurementCookie([NotNull] List<long> data)
			{
				Data = data;
				Stopwatch = Stopwatch.StartNew();
			}

			public void Dispose()
			{
				Stopwatch.Stop();
				Data.Add(Stopwatch.ElapsedMilliseconds);
			}
		}
	}
}
