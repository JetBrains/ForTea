using System;
using System.Collections.Generic;
using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Psi.Resolve.Macros;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Descriptions;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Interrupt;
using GammaJul.ForTea.Core.Tree;
using GammaJul.ForTea.Core.Tree.Impl;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.Diagnostics;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting
{
	public abstract class T4CSharpCodeGenerationInfoCollectorBase : IRecursiveElementProcessor
	{
		#region Properties
		[NotNull]
		private IT4File File { get; }

		[NotNull]
		private T4DirectiveInfoManager Manager { get; }

		[NotNull]
		private T4TreeNavigator Navigator { get; }

		[NotNull]
		private T4EncodingsManager EncodingsManager { get; }

		[NotNull]
		private T4IncludeGuard Guard { get; }

		[NotNull, ItemNotNull]
		private Stack<T4CSharpCodeGenerationIntermediateResult> Results { get; }

		private bool HasSeenTemplateDirective { get; set; }

		[NotNull]
		protected T4CSharpCodeGenerationIntermediateResult Result => Results.Peek();
		#endregion Properties

		protected T4CSharpCodeGenerationInfoCollectorBase(
			[NotNull] IT4File file,
			[NotNull] ISolution solution
		)
		{
			File = file;
			Results = new Stack<T4CSharpCodeGenerationIntermediateResult>();
			Guard = new T4IncludeGuard();
			Manager = solution.GetComponent<T4DirectiveInfoManager>();
			Navigator = solution.GetComponent<T4TreeNavigator>();
			EncodingsManager = solution.GetComponent<T4EncodingsManager>();
		}

		[NotNull]
		public T4CSharpCodeGenerationIntermediateResult Collect()
		{
			Results.Push(new T4CSharpCodeGenerationIntermediateResult(File, Interrupter));
			Guard.StartProcessing(File.GetSourceFile().NotNull());
			File.ProcessDescendants(this);
			string suffix = Result.State.ProduceBeforeEof();
			if (!suffix.IsNullOrEmpty()) AppendTransformation(suffix);
			Guard.EndProcessing();
			return Results.Pop();
		}

		#region Interface Members
		public bool InteriorShouldBeProcessed(ITreeNode element) => element is IT4Include;

		public void ProcessBeforeInterior(ITreeNode element)
		{
			if (!(element is IT4Include include)) return;
			Results.Push(new T4CSharpCodeGenerationIntermediateResult(File, Interrupter));
			var sourceFile = include.Path.Resolve();
			if (sourceFile == null)
			{
				var target = Navigator.FindIncludeValue(include) ?? element;
				var data = T4FailureRawData.FromElement(target, $"Unresolved include: {target}");
				Interrupter.InterruptAfterProblem(data);
				Guard.StartProcessing(null);
				return;
			}

			if (include.Once && Guard.HasSeenFile(sourceFile)) return;
			if (!Guard.CanProcess(sourceFile))
			{
				var target = Navigator.FindIncludeValue(include) ?? element;
				var data = T4FailureRawData.FromElement(target, "Recursion in includes");
				Interrupter.InterruptAfterProblem(data);
				Guard.StartProcessing(sourceFile);
				return;
			}

			var resolved = include.Path.ResolveT4File(Guard).NotNull();
			Guard.StartProcessing(sourceFile);
			resolved.ProcessDescendants(this);
		}

		public void ProcessAfterInterior(ITreeNode element)
		{
			switch (element)
			{
				case IT4Include include:
					string suffix = Result.State.ProduceBeforeEof();
					if (!suffix.IsNullOrEmpty()) AppendTransformation(suffix);
					Guard.TryEndProcessing(include.Path.Resolve());
					var intermediateResults = Results.Pop();
					Result.Append(intermediateResults);
					return; // Do not advance state here
				case IT4Directive directive:
					AppendRemainingMessage(element);
					HandleDirective(directive);
					break;
				case IT4CodeBlock codeBlock:
					AppendRemainingMessage(element);
					HandleCodeBlock(codeBlock);
					break;
				case IT4Token token:
					Result.State.ConsumeToken(token);
					break;
			}

			Result.AdvanceState(element);
		}

		public bool ProcessingIsFinished
		{
			get
			{
				InterruptableActivityCookie.CheckAndThrow();
				return false;
			}
		}
		#endregion Interface Members

		#region Utils
		/// <summary>Handles a directive in the tree.</summary>
		/// <param name="directive">The directive.</param>
		private void HandleDirective([NotNull] IT4Directive directive)
		{
			if (directive.IsSpecificDirective(Manager.Import))
				HandleImportDirective(directive);
			else if (directive.IsSpecificDirective(Manager.Template))
				HandleTemplateDirective(directive);
			else if (directive.IsSpecificDirective(Manager.Parameter))
				HandleParameterDirective(directive);
			else if (directive.IsSpecificDirective(Manager.Output))
				HandleOutputDirective(directive);
		}

		/// <summary>
		/// Handles a code block: depending of whether it's a feature or transform text result,
		/// it is not added to the same part of the C# file.
		/// </summary>
		/// <param name="codeBlock">The code block.</param>
		private void HandleCodeBlock([NotNull] IT4CodeBlock codeBlock)
		{
			if (!(codeBlock.GetCodeToken() is T4Code codeToken)) return;
			switch (codeBlock)
			{
				case T4ExpressionBlock _:
					if (Result.FeatureStarted) Result.AppendFeature(new T4ExpressionDescription(codeToken));
					else Result.AppendTransformation(new T4ExpressionDescription(codeToken));
					break;
				case T4FeatureBlock _:
					Result.AppendFeature(new T4CodeDescription(codeToken));
					break;
				default:
					Result.AppendTransformation(new T4CodeDescription(codeToken));
					break;
			}
		}

		private void HandleOutputDirective([NotNull] IT4Directive directive) =>
			Result.Encoding = EncodingsManager.FindEncoding(directive, Interrupter);

		/// <summary>Handles an import directive, equivalent of an using directive in C#.</summary>
		/// <param name="directive">The import directive.</param>
		private void HandleImportDirective([NotNull] IT4Directive directive)
		{
			var description = T4ImportDescription.FromDirective(directive, Manager);
			if (description == null) return;
			Result.Append(description);
		}

		/// <summary>
		/// Handles a template directive,
		/// determining if we should output a Host property
		/// and use a base class.
		/// </summary>
		/// <param name="directive">The template directive.</param>
		private void HandleTemplateDirective([NotNull] IT4Directive directive)
		{
			if (HasSeenTemplateDirective) return;
			HasSeenTemplateDirective = true;
			string hostSpecific = directive.GetAttributeValue(Manager.Template.HostSpecificAttribute.Name);
			if (bool.TrueString.Equals(hostSpecific, StringComparison.OrdinalIgnoreCase)) Result.RequireHost();

			(ITreeNode classNameToken, string className) =
				directive.GetAttributeValueIgnoreOnlyWhitespace(Manager.Template.InheritsAttribute.Name);
			if (classNameToken != null && className != null)
				Result.CollectedBaseClass.AppendMapped(className, classNameToken.GetTreeTextRange());
		}

		/// <summary>Handles a parameter directive, outputting an extra property.</summary>
		/// <param name="directive">The parameter directive.</param>
		private void HandleParameterDirective([NotNull] IT4Directive directive)
		{
			var description = T4ParameterDescription.FromDirective(directive, Manager);
			if (description == null) return;
			Result.Append(description);
		}

		private void AppendRemainingMessage([NotNull] ITreeNode lookahead)
		{
			if (lookahead is IT4Token) return;
			string produced = Result.State.Produce(lookahead);
			if (produced.IsNullOrEmpty()) return;
			AppendTransformation(produced);
		}
		#endregion Utils

		protected abstract void AppendTransformation([NotNull] string message);
		protected abstract IT4CodeGenerationInterrupter Interrupter { get; }
	}
}
