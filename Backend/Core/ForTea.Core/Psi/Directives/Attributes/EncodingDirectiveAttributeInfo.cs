using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting;
using JetBrains.Annotations;
using JetBrains.DataStructures;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Psi.Directives.Attributes
{
	public class EncodingDirectiveAttributeInfo : DirectiveAttributeInfo
	{
		[NotNull, ItemNotNull]
		private Lazy<JetHashSet<string>> Encodings { get; }

		[NotNull, ItemNotNull]
		private Lazy<JetImmutableArray<string>> CompletionValues { get; }

		public EncodingDirectiveAttributeInfo(
			DirectiveAttributeOptions options
		) : base("encoding", options)
		{
			Encodings = Lazy.Of(CreateEncodings, true);
			CompletionValues = Lazy.Of(CreateCompletionValues, true);
		}

		public override bool IsValid(string value) =>
			T4EncodingsManager.IsCodePage(value) || T4EncodingsManager.IsEncodingName(value);

		public override JetImmutableArray<string> IntelliSenseValues =>
			CompletionValues.Value;

		[NotNull]
		private static JetHashSet<string> CreateEncodings() =>
			Encoding.GetEncodings().Select(info => info.Name).ToJetHashSet(StringComparer.OrdinalIgnoreCase);

		private JetImmutableArray<string> CreateCompletionValues() =>
			Encodings.Value.ToImmutableArray();
	}
}
