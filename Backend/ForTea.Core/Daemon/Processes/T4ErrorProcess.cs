using System;
using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Tree;
using GammaJul.ForTea.Core.Tree.Impl;
using JetBrains.Annotations;
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
		[NotNull] private readonly T4DirectiveInfoManager _directiveInfoManager;
		[CanBeNull] private IT4FeatureBlock _lastFeature;
		private bool _gotFeature;
		private bool _gotLastFeature;
		private bool _inLastFeature;
		private bool _afterLastFeatureErrorAdded;

		public override void ProcessBeforeInterior(ITreeNode element)
		{
			switch (element)
			{
				case T4StatementBlock statementBlock when _gotFeature:
					AddHighlighting(element.GetHighlightingRange(),
						new StatementAfterFeatureHighlighting(statementBlock));
					return;

				case IT4Directive directive:
					ProcessDirective(directive);
					break;

				case T4FeatureBlock _:
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
			AddHighlighting(range, new AfterLastFeatureHighlighting(element));
			_afterLastFeatureErrorAdded = true;
		}

		private void ProcessAttributeValue([NotNull] IT4AttributeValue valueNode)
		{
			if (!(valueNode.Parent is IT4DirectiveAttribute attribute))
				return;

			if (!(attribute.Parent is IT4Directive directive))
				return;

			var attributeInfo = _directiveInfoManager.GetDirectiveByName(directive.Name.GetText())
				?.GetAttributeByName(attribute.Name.GetText());
			if (attributeInfo?.IsValid(valueNode.GetText()) != false)
				return;

			AddHighlighting(valueNode.GetHighlightingRange(),
				new InvalidAttributeValueHighlighting(valueNode, attributeInfo));
		}

		private void ProcessDirective([NotNull] IT4Directive directive)
		{
			var nameToken = directive.Name;
			if (nameToken == null)
				return;

			DirectiveInfo directiveInfo = _directiveInfoManager.GetDirectiveByName(nameToken.GetText());
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
				var highlighting = new MissingRequiredAttributeHighlighting(nameToken, attributeInfo.Name);
				AddHighlighting(range, highlighting);
			}

			// Assembly attributes in preprocessed templates are useless.
			if (directiveInfo == _directiveInfoManager.Assembly &&
			    DaemonProcess.SourceFile.ToProjectFile().IsPreprocessedT4Template())
				AddHighlighting(directive.GetHighlightingRange(), new IgnoredAssemblyDirectiveHighlighting(directive));
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

		/// <summary>Initializes a new instance of the <see cref="T4DaemonStageProcessBase"/> class.</summary>
		/// <param name="file">The associated T4 file.</param>
		/// <param name="daemonProcess">The associated daemon process.</param>
		/// <param name="directiveInfoManager">An instance of <see cref="T4DirectiveInfoManager"/>.</param>
		public T4ErrorProcess(
			[NotNull] IT4File file,
			[NotNull] IDaemonProcess daemonProcess,
			[NotNull] T4DirectiveInfoManager directiveInfoManager
		) : base(file, daemonProcess) => _directiveInfoManager = directiveInfoManager;

		protected override void AnalyzeFile(IT4File file)
		{
			var outputDirective = file.GetDirectives().FirstOrDefault(directive =>
				directive.IsSpecificDirective(_directiveInfoManager.Output));
			var extensionAttribute = outputDirective
				?.GetAttributes(_directiveInfoManager.Output.ExtensionAttribute.Name)
				?.FirstOrDefault();
			if (extensionAttribute == null)
			{
				// TODO: show notification
			}
		}
	}
}
