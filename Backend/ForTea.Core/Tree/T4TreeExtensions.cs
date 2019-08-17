using System;
using System.Collections.Generic;
using System.Linq;
using GammaJul.ForTea.Core.Parsing;
using GammaJul.ForTea.Core.Psi.Directives;
using GammaJul.ForTea.Core.Psi.Resolve.Macros;
using GammaJul.ForTea.Core.Tree.Impl;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.DocumentModel;
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
			return directive?.Attributes.Where(it => it.Name.GetText() == attributeName)?.FirstOrDefault()?.Value;
		}

		[CanBeNull]
		public static string GetAttributeValue(
			[CanBeNull] this IT4Directive directive,
			[CanBeNull] string attributeName
		) => directive.GetAttributeValueToken(attributeName)?.GetText();

		public static Pair<ITreeNode, string> GetAttributeValueIgnoreOnlyWhitespace(
			[CanBeNull] this IT4Directive directive,
			[CanBeNull] string attributeName
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

		[ContractAnnotation("directive:null => false")]
		public static bool IsSpecificDirective(
			[CanBeNull] this IT4Directive directive,
			[CanBeNull] DirectiveInfo directiveInfo
		) => directive != null &&
		     directiveInfo?.Name.Equals(directive.Name.GetText(), StringComparison.OrdinalIgnoreCase) == true;

		public static IEnumerable<IT4Directive> GetDirectives([NotNull] this IT4File file) =>
			file.BlocksEnumerable.OfType<IT4Directive>();

		[NotNull]
		public static IEnumerable<IT4Directive> GetDirectives(
			[NotNull] this IT4File file,
			[NotNull] DirectiveInfo directiveInfo
		) => file.GetDirectives().Where(d =>
			directiveInfo.Name.Equals(d.Name.GetText(), StringComparison.OrdinalIgnoreCase));

		[CanBeNull]
		private static string GetSortValue(
			[NotNull] IT4Directive directive,
			[CanBeNull] DirectiveInfo directiveInfo,
			[NotNull] T4DirectiveInfoManager directiveInfoManager
		)
		{
			if (directiveInfo == directiveInfoManager.Assembly)
				return directive.GetAttributeValue(directiveInfoManager.Assembly.NameAttribute.Name);
			if (directiveInfo == directiveInfoManager.Import)
				return directive.GetAttributeValue(directiveInfoManager.Import.NamespaceAttribute.Name);
			if (directiveInfo == directiveInfoManager.Parameter)
				return directive.GetAttributeValue(directiveInfoManager.Parameter.NameAttribute.Name);
			return null;
		}

		/// <summary>Finds an anchor for a newly created directive inside a list of existing directives.</summary>
		/// <param name="newDirective">The directive to add.</param>
		/// <param name="existingDirectives">The existing directives.</param>
		/// <param name="directiveInfoManager">An instance of <see cref="T4DirectiveInfoManager"/>.</param>
		/// <returns>A pair indicating the anchor (can be null) and its relative position.</returns>
		public static Pair<IT4Directive, BeforeOrAfter> FindAnchor(
			[NotNull] this IT4Directive newDirective,
			[NotNull] IT4Directive[] existingDirectives,
			[NotNull] T4DirectiveInfoManager directiveInfoManager
		)
		{
			// no anchor
			if (existingDirectives.Length == 0)
				return Pair.Of((IT4Directive) null, BeforeOrAfter.Before);

			// directive name should never be null, but you never know
			string newName = newDirective.Name.GetText();
			if (String.IsNullOrEmpty(newName))
				return Pair.Of(existingDirectives.Last(), BeforeOrAfter.After);

			var lastDirectiveByName = new Dictionary<string, IT4Directive>(StringComparer.OrdinalIgnoreCase);
			DirectiveInfo directiveInfo = directiveInfoManager.GetDirectiveByName(newName);
			string newsortValue = GetSortValue(newDirective, directiveInfo, directiveInfoManager);

			foreach (var existingDirective in existingDirectives)
			{
				string existingName = existingDirective.Name?.GetText();
				if (existingName == null) continue;

				lastDirectiveByName[existingName] = existingDirective;

				// directive of the same type as the new one:
				// if the new directive comes alphabetically before the existing one, we got out anchor
				if (!string.Equals(existingName, newName, StringComparison.OrdinalIgnoreCase)) continue;
				string existingSortValue = GetSortValue(existingDirective, directiveInfo, directiveInfoManager);
				if (string.Compare(newsortValue, existingSortValue, StringComparison.OrdinalIgnoreCase) < 0)
					return Pair.Of(existingDirective, BeforeOrAfter.Before);
			}

			// no anchor being alphabetically after the new directive was found:
			// the last directive of the same type will be used as an anchor
			if (lastDirectiveByName.TryGetValue(newName, out IT4Directive lastDirective))
				return Pair.Of(lastDirective, BeforeOrAfter.After);

			// there was no directive of the same type as the new one
			// the anchor will be the last directive of the type just before (determined by the position in DirectiveInfo.AllDirectives)
			if (directiveInfo == null)
				// we don't know the directive name (shouldn't happen), use the last directive as an anchor
				return Pair.Of(existingDirectives.Last(), BeforeOrAfter.After);
			int index = directiveInfoManager.AllDirectives.IndexOf(directiveInfo) - 1;
			while (index >= 0)
			{
				if (lastDirectiveByName.TryGetValue(directiveInfoManager.AllDirectives[index].Name,
					out lastDirective))
					return Pair.Of(lastDirective, BeforeOrAfter.After);
				--index;
			}

			return Pair.Of(existingDirectives.First(), BeforeOrAfter.Before);
		}

		[NotNull]
		public static IEnumerable<IT4DirectiveAttribute> GetAttributes(
			[NotNull] this IT4Directive directive,
			[NotNull] string name
		) => directive.Attributes.Where(it => it.Name.GetText() == name);

		[NotNull, ItemNotNull]
		public static IEnumerable<IT4File> GetIncludedFilesRecursive([NotNull] this IT4File file,
			[NotNull] T4IncludeGuard guard)
		{
			var sourceFile = file.GetSourceFile();
			if (sourceFile == null || guard.CanProcess(sourceFile)) yield break;
			guard.StartProcessing(sourceFile);
			var includedFiles = file.Blocks.OfType<IT4IncludeDirective>()
				.Select(include => include.Path.ResolveT4File(guard))
				.Where(resolution => resolution != null);
			foreach (var includedFile in includedFiles)
			{
				yield return includedFile;
				foreach (var recursiveInclude in includedFile.GetIncludedFilesRecursive(guard))
				{
					yield return recursiveInclude;
				}
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

		/// <summary>Adds a directive to a <see cref="IT4File"/> at an optimal location in the directive list.</summary>
		/// <param name="t4File">The <see cref="IT4File"/> to add the directive to.</param>
		/// <param name="directive">The directive to add.</param>
		/// <param name="directiveInfoManager">A <see cref="T4DirectiveInfoManager"/> used to determine the best location of the directive.</param>
		/// <returns>A new instance of <see cref="IT4Directive"/>, representing <paramref name="directive"/> in the T4 file.</returns>
		[NotNull]
		public static IT4Directive AddDirective(
			[NotNull] this IT4File t4File,
			[NotNull] IT4Directive directive,
			[NotNull] T4DirectiveInfoManager directiveInfoManager
		)
		{
			(IT4Directive anchor, BeforeOrAfter beforeOrAfter) =
				directive.FindAnchor(t4File.GetDirectives().ToArray(), directiveInfoManager);

			if (anchor == null)
				return t4File.AddDirective(directive);

			return beforeOrAfter == BeforeOrAfter.Before
				? t4File.AddDirectiveBefore(directive, anchor)
				: t4File.AddDirectiveAfter(directive, anchor);
		}

		public static IT4Directive AddDirective([NotNull] this IT4File file, [NotNull] IT4Directive directive)
		{
			IT4Directive anchor = file.GetDirectives().LastOrDefault();
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

		public static IT4Directive AddDirectiveBefore(this IT4File file, IT4Directive directive, IT4Directive anchor)
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

		/// <summary>Adds a new directive after an existing one.</summary>
		/// <param name="directive">The directive to add.</param>
		/// <param name="anchor">The existing directive where <paramref name="directive"/> will be placed after.</param>
		/// <returns>A new instance of <see cref="IT4Directive"/>, representing <paramref name="directive"/> in the T4 file.</returns>
		public static IT4Directive AddDirectiveAfter([NotNull] this IT4File file, IT4Directive directive,
			IT4Directive anchor)
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
		public static IT4FeatureBlock AddFeatureBlock([NotNull] this IT4File file,
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
	}
}
