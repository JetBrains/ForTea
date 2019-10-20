using System;
using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.Parsing;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Psi.Directives.Attributes;
using GammaJul.ForTea.Core.Psi.Utils;
using GammaJul.ForTea.Core.Psi.Utils.Impl;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Tree
{
	public static class T4TreeExtensions
	{
		[CanBeNull]
		public static ITreeNode GetAttributeValueToken(
			[CanBeNull] this IT4Directive directive,
			[CanBeNull] string attributeName
		)
		{
			if (string.IsNullOrEmpty(attributeName)) return null;
			return directive
				?.Attributes
				.Where(it => string.Equals(it.Name.GetText(), attributeName, StringComparison.OrdinalIgnoreCase))
				?.FirstOrDefault()
				?.Value;
		}

		[CanBeNull]
		public static string GetAttributeValueByName(
			[NotNull] this IT4Directive directive,
			[NotNull] string attributeName
		) => directive.GetAttributeValueToken(attributeName)?.GetText();

		public static Pair<ITreeNode, string> GetAttributeValueIgnoreOnlyWhitespace(
			[NotNull] this IT4Directive directive,
			[NotNull] string attributeName
		)
		{
			var valueToken = directive.GetAttributeValueToken(attributeName);
			if (valueToken == null)
				return new Pair<ITreeNode, string>();

			string value = valueToken.GetText();
			if (value.IsNullOrWhitespace())
				return new Pair<ITreeNode, string>();

			return new Pair<ITreeNode, string>(valueToken, value);
		}

		[NotNull]
		public static IEnumerable<IT4Directive> GetDirectives(
			[NotNull] this IT4File file,
			[NotNull] DirectiveInfo directiveInfo
		) => file.Blocks.OfType<IT4Directive>().Where(d =>
			directiveInfo.Name.Equals(d.Name?.GetText(), StringComparison.OrdinalIgnoreCase));

		[NotNull]
		public static IEnumerable<IT4DirectiveAttribute> GetAttributes(
			[NotNull] this IT4Directive directive,
			[NotNull] DirectiveAttributeInfo info
		) => directive.Attributes.Where(it =>
			string.Equals(it.Name.GetText(), info.Name, StringComparison.OrdinalIgnoreCase));

		[CanBeNull]
		public static IT4DirectiveAttribute GetFirstAttribute(
			[NotNull] this IT4Directive directive,
			[NotNull] DirectiveAttributeInfo info
		) => directive.Attributes.FirstOrDefault(it =>
			string.Equals(it.Name.GetText(), info.Name, StringComparison.OrdinalIgnoreCase));

		[NotNull, ItemNotNull]
		public static IEnumerable<IT4File> GetThisAndIncludedFilesRecursive([NotNull] this IT4File file)
		{
			var guard = new T4ContextTrackingIncludeGuard();
			return file.GetThisAndIncludedFilesRecursive(guard);
		}

		[NotNull, ItemNotNull]
		private static IEnumerable<IT4File> GetThisAndIncludedFilesRecursive(
			[NotNull] this IT4File file,
			[NotNull] IT4IncludeGuard<IPsiSourceFile> guard
		)
		{
			yield return file;
			var sourceFile = file.GetSourceFile();
			if (sourceFile == null || !guard.CanProcess(sourceFile)) yield break;
			guard.StartProcessing(sourceFile);
			var includedFiles = file.Blocks.OfType<IT4IncludeDirective>()
				.Select(include => include.Path.ResolveT4File(guard))
				.Where(resolution => resolution != null);
			foreach (var recursiveInclude in includedFiles
				.SelectMany(includedFile => includedFile.GetThisAndIncludedFilesRecursive(guard))
			)
			{
				yield return recursiveInclude;
			}
		}

		/// <summary>Gets a T4 block containing a specified C# node.</summary>
		/// <typeparam name="T">The type of expected T4 container node.</typeparam>
		/// <param name="cSharpNode">The C# node whose T4 container will be retrieved.</param>
		/// <returns>An instance of <see cref="T"/>, or <c>null</c> if no container for <paramref name="cSharpNode"/> can be found.</returns>
		[CanBeNull]
		public static T GetT4ContainerFromCSharpNode<T>([CanBeNull] this ITreeNode cSharpNode)
			where T : ITreeNode
		{
			ISecondaryRangeTranslator secondaryRangeTranslator =
				(cSharpNode?.GetContainingFile() as IFileImpl)?.SecondaryRangeTranslator;
			if (secondaryRangeTranslator == null)
				return default;

			DocumentRange range = cSharpNode.GetDocumentRange();
			if (!range.IsValid())
				return default;

			ITreeNode t4Node = secondaryRangeTranslator.OriginalFile.FindNodeAt(range);
			if (t4Node == null)
				return default;

			return t4Node.GetContainingNode<T>(true);
		}

		[NotNull]
		public static IT4Directive AddDirective([NotNull] this IT4File file, [NotNull] IT4Directive directive)
		{
			IT4Directive anchor = file.Blocks.OfType<IT4Directive>().LastOrDefault();
			if (anchor != null)
				return file.AddDirectiveAfter(directive, anchor);

			using (WriteLockCookie.Create(file.IsPhysical()))
			{
				directive = file.FirstChild != null
					? ModificationUtil.AddChildBefore(file.FirstChild, directive)
					: ModificationUtil.AddChild(file, directive);
				ModificationUtil.AddChildAfter(directive, T4TokenNodeTypes.NEW_LINE.CreateLeafElement());
				return directive;
			}
		}

		[NotNull]
		public static IT4Directive AddDirectiveBefore(
			[NotNull] this IT4File file,
			IT4Directive directive,
			[NotNull] IT4Directive anchor
		)
		{
			using (WriteLockCookie.Create(file.IsPhysical()))
			{
				directive = ModificationUtil.AddChildBefore(anchor, directive);

				// if the directive was inserted between a new line (or the file start) and the anchor, add another new line after
				// the directive so that both directives have new lines after them
				if (directive.PrevSibling == null || directive.PrevSibling.GetTokenType() == T4TokenNodeTypes.NEW_LINE)
					ModificationUtil.AddChildAfter(directive, T4TokenNodeTypes.NEW_LINE.CreateLeafElement());

				return directive;
			}
		}

		[NotNull]
		public static IT4Directive AddDirectiveAfter(
			[NotNull] this IT4File file,
			[NotNull] IT4Directive directive,
			[NotNull] IT4Directive anchor
		)
		{
			using (WriteLockCookie.Create(file.IsPhysical()))
			{
				directive = ModificationUtil.AddChildAfter(anchor, directive);

				// if the directive was inserted between the anchor and a new line, add another new line before
				// the directive so that both directives have new lines after them
				var sibling = directive.NextSibling;
				if (sibling != null && sibling.GetTokenType() == T4TokenNodeTypes.NEW_LINE)
					ModificationUtil.AddChildBefore(directive, T4TokenNodeTypes.NEW_LINE.CreateLeafElement());

				return directive;
			}
		}

		public static void RemoveDirective([NotNull] this IT4File file, [CanBeNull] IT4Directive directive)
		{
			if (directive == null) return;
			using (WriteLockCookie.Create(file.IsPhysical()))
			{
				// remove the optional end line after the directive
				var sibling = directive.NextSibling;
				var endNode = sibling?.GetTokenType() == T4TokenNodeTypes.NEW_LINE ? sibling : directive;
				ModificationUtil.DeleteChildRange(directive, endNode);
			}
		}

		[NotNull]
		public static IT4FeatureBlock AddFeatureBlock(
			[NotNull] this IT4File file,
			[NotNull] IT4FeatureBlock featureBlock)
		{
			var anchor = file.Blocks.OfType<IT4FeatureBlock>().LastOrDefault();
			using (WriteLockCookie.Create(file.IsPhysical()))
			{
				if (anchor == null)
					return ModificationUtil.AddChild(file, featureBlock);
				return ModificationUtil.AddChildAfter(anchor, featureBlock);
			}
		}

		[NotNull]
		public static IT4DirectiveAttribute AddAttribute(
			[NotNull] this IT4Directive directive,
			[NotNull] IT4DirectiveAttribute attribute
		)
		{
			using (WriteLockCookie.Create(directive.IsPhysical()))
			{
				var lastNode = directive.LastChild;
				Assertion.AssertNotNull(lastNode, "lastNode != null");

				var anchor = lastNode.GetTokenType() == T4TokenNodeTypes.BLOCK_END ? lastNode.PrevSibling : lastNode;
				Assertion.AssertNotNull(anchor, "anchor != null");
				bool addSpaceAfter = anchor.GetTokenType() == T4TokenNodeTypes.WHITE_SPACE;
				bool addSpaceBefore = !addSpaceAfter;

				if (addSpaceBefore)
					anchor = ModificationUtil.AddChildAfter(anchor, T4TokenNodeTypes.WHITE_SPACE.CreateLeafElement());

				IT4DirectiveAttribute result = ModificationUtil.AddChildAfter(anchor, attribute);

				if (addSpaceAfter)
					ModificationUtil.AddChildAfter(result, T4TokenNodeTypes.WHITE_SPACE.CreateLeafElement());

				return result;
			}
		}

		[NotNull]
		public static IEnumerable<TParent> GetParentsOfType<TParent>([NotNull] this ITreeNode node)
			where TParent : class, ITreeNode
		{
			for (; node != null; node = node.Parent)
			{
				if (!(node is TParent obj)) continue;
				yield return obj;
			}
		}

		[CanBeNull]
		public static TParent GetParentOfType<TParent>([NotNull] this ITreeNode node)
			where TParent : class, ITreeNode
		{
			for (; node != null; node = node.Parent)
			{
				if (!(node is TParent obj)) continue;
				return obj;
			}

			return null;
		}
	}
}
