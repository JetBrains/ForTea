using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using GammaJul.ForTea.Core.Parsing.Lexing;
using GammaJul.ForTea.Core.Parsing.Parser;
using GammaJul.ForTea.Core.Psi.FileType;
using GammaJul.ForTea.Core.Test;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ForTea.Tests.Mock;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.TestFramework;
using JetBrains.Util;
using NUnit.Framework;

namespace JetBrains.ForTea.Tests.Psi
{
	[TestFileExtension(T4FileExtensions.MainExtension)]
	public sealed class T4CloningParserStressTest : BaseTestWithTextControl
	{
		protected override string RelativeTestDataPath => @"Psi\Parser";
		private const int TEST_DATA_SIZE = 800;
		private const int MEASUREMENT_COUNT = 200;

		private const string TEST_DATA_TEXT =
			"<#@ template language=\"C#\" #>\n<#@ include file=\"%VAR%/$(Foo)/dummy.ttinclude\" #>";

		const string INCLUDE_TEXT =
			"<#@ using namespace=\"System.Text\" #>\n<#@ output extension=\"cs\" #>\n<#+\n    public void Foo()\n    {\n    }\n#>";

		[Test]
		public void StressTest() => DoTestSolution("StressTest.tt");

		protected override void DoTest(Lifetime lifetime)
		{
			var realSourceFile = Solution.GetAllProjects().ElementAt(2).GetAllProjectFiles().Single().ToSourceFile();
			Console.WriteLine("Generating data...");
			string text = GenerateLargeString(TEST_DATA_SIZE);
			Console.WriteLine("Warming up...");
			WarmUpJit(realSourceFile);
			GC.Collect();
			var parseTimes = new List<long>();
			var cloneTimes = new List<long>();
			Console.WriteLine("Measuring...");
			for (int i = 0; i < MEASUREMENT_COUNT; i += 1)
			{
				IFile file;
				using (new MyTimeMeasurementCookie(parseTimes))
				{
					file = T4ParserExposer
						.Create(text, new T4ParsingMockIncludeParser(INCLUDE_TEXT), realSourceFile)
						.ParseFile();
					T4CloningParserTestUtils.InitializeResolvePaths(((IT4File) file)!);
				}

				using (new MyTimeMeasurementCookie(cloneTimes))
				{
					new T4CloningParser(new T4MockPsiFileProvider(file!), null, T4DocumentLexerSelector.Instance)
						.ParseFile();
				}
			}

			Console.WriteLine("Done!");
			Console.WriteLine();
			Console.WriteLine($"Test data size: {TEST_DATA_SIZE * (TEST_DATA_TEXT.Length + 1) + TEST_DATA_SIZE * INCLUDE_TEXT.Length}");
			Console.WriteLine($"Number of measurements: {MEASUREMENT_COUNT}");
			Console.WriteLine("{0} {1} {2}", "operation", "mean", "stddev");
			Console.WriteLine("{0} {1} {2}", "parse", parseTimes.Average(), StdDev(parseTimes));
			Console.WriteLine("{0} {1} {2}", "clone", cloneTimes.Average(), StdDev(cloneTimes));
			File.WriteAllText(@"C:\w\Draft\parse.csv", parseTimes.AggregateString(","));
			File.WriteAllText(@"C:\w\Draft\clone.csv", cloneTimes.AggregateString(","));
		}

		private static double StdDev([NotNull] ICollection<long> data)
		{
			if (data.Count <= 1) return 0;
			double avg = data.Average();
			return Math.Sqrt(data.Sum(d => (d - avg) * (d - avg)) / data.Count);
		}

		private static void WarmUpJit(IPsiSourceFile realSourceFile)
		{
			string text = GenerateLargeString(100);
			for (int i = 0; i < 10; i += 1)
			{
				var dummyData = new List<long>();
				using var cookie = new MyTimeMeasurementCookie(dummyData);
				var file = T4ParserExposer
					.Create(text, new T4ParsingMockIncludeParser(INCLUDE_TEXT), realSourceFile)
					.ParseFile();
				T4CloningParserTestUtils.InitializeResolvePaths(((IT4File) file)!);
				new T4CloningParser(new T4MockPsiFileProvider(file!), null, T4DocumentLexerSelector.Instance)
					.ParseFile();
			}
		}

		[NotNull]
		private static string GenerateLargeString(int numberOfRepetitions)
		{
			var lines = new List<string>();
			for (int i = 0; i < numberOfRepetitions; i += 1)
			{
				lines.Add(TEST_DATA_TEXT);
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
