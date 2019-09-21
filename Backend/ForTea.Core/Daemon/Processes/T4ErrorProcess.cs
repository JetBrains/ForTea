using System;
using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.TemplateProcessing.Services;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Daemon.Processes
{
	internal sealed class T4ErrorProcess : T4DaemonStageProcessBase
	{
		[CanBeNull] private IT4FeatureBlock _lastFeature;
		private bool _gotFeature;
		private bool _gotLastFeature;
		private bool _inLastFeature;
		private bool _afterLastFeatureErrorAdded;

		[NotNull]
		private IT4TemplateKindProvider TemplateDataManager { get; }

		public override void ProcessBeforeInterior(ITreeNode element)
		{
			switch (element)
			{
				case IT4StatementBlock statementBlock when _gotFeature:
					AddHighlighting(element.GetHighlightingRange(),
						new StatementAfterFeatureError(statementBlock));
					return;

				case IT4Directive directive:
					ProcessDirective(directive);
					break;

				case IT4FeatureBlock _:
					_gotFeature = true;
					if (element == _lastFeature)
					{
						_gotLastFeature = true;
						_inLastFeature = true;
						return;
					}

					break;
			}

			// verify that a directive attribute value is valid
			if (element is IT4AttributeValue value)
			{
				ProcessAttributeValue(value);
				return;
			}

			// can't have anything after the last feature block
			if (!_gotLastFeature || _inLastFeature || _afterLastFeatureErrorAdded)
				return;

			TokenNodeType tokenType = element.GetTokenType();
			if (tokenType?.IsWhitespace == true)
				return;

			// highlight from just after the last feature to the end of the document
			DocumentRange range = element.GetHighlightingRange().SetEndTo(File.GetDocumentRange().EndOffset);
			AddHighlighting(range, new TextAfterFeatureError(element));
			_afterLastFeatureErrorAdded = true;
		}

		private void ProcessAttributeValue([NotNull] IT4AttributeValue valueNode)
		{
			if (!(valueNode.Parent is IT4DirectiveAttribute attribute))
				return;

			if (!(attribute.Parent is IT4Directive directive))
				return;

			var attributeInfo = T4DirectiveInfoManager
				.GetDirectiveByName(directive.Name.GetText())
				?.GetAttributeByName(attribute.Name.GetText());
			if (attributeInfo?.IsValid(valueNode.GetText()) != false) return;

			AddHighlighting(valueNode.GetHighlightingRange(), new InvalidAttributeValueError(valueNode));
		}

		private void ProcessDirective([NotNull] IT4Directive directive)
		{
			var nameToken = directive.Name;
			if (nameToken == null)
				return;

			DirectiveInfo directiveInfo = T4DirectiveInfoManager.GetDirectiveByName(nameToken.GetText());
			if (directiveInfo == null)
				return;

			// Notify of missing required attributes.
			IEnumerable<string> attributeNames = directive.Attributes.SelectNotNull(attr => attr.Name.GetText());
			var hashSet = new JetHashSet<string>(attributeNames, StringComparer.OrdinalIgnoreCase);
			var infos = directiveInfo
				.SupportedAttributes
				.Where(attributeInfo => attributeInfo.IsRequired && !hashSet.Contains(attributeInfo.Name));
			foreach (var attributeInfo in infos)
			{
				var range = nameToken.GetHighlightingRange();
				var highlighting = new MissingRequiredAttributeError(nameToken, attributeInfo.Name);
				AddHighlighting(range, highlighting);
			}

			// Assembly attributes in preprocessed templates are useless.
			if (!(directive is IT4AssemblyDirective assemblyDirective)) return;
			var projectFile = DaemonProcess.SourceFile.ToProjectFile().NotNull();
			if (!TemplateDataManager.IsPreprocessedTemplate(projectFile)) return;
			AddHighlighting(directive.GetHighlightingRange(), new IgnoredAssemblyDirectiveWarning(assemblyDirective));
		}

		public override void ProcessAfterInterior(ITreeNode element)
		{
			if (element == _lastFeature)
				_inLastFeature = false;
		}

		public override void Execute(Action<DaemonStageResult> commiter)
		{
			_lastFeature = File.Blocks.OfType<IT4FeatureBlock>().LastOrDefault();
			base.Execute(commiter);
		}

		public T4ErrorProcess(
			[NotNull] IT4File file,
			[NotNull] IDaemonProcess daemonProcess,
			[NotNull] IT4TemplateKindProvider templateDataManager
		) : base(file, daemonProcess) => TemplateDataManager = templateDataManager;

		protected override void AnalyzeFile(IT4File file)
		{
			var outputDirective = file.Blocks.OfType<IT4OutputDirective>().FirstOrDefault();
			var extensionAttribute = outputDirective
				?.GetFirstAttribute(T4DirectiveInfoManager.Output.ExtensionAttribute);
			if (extensionAttribute == null)
			{
				// TODO: show notification
			}
		}
	}
}
