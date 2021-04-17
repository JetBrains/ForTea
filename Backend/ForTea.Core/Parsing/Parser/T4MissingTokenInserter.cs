using GammaJul.ForTea.Core.Parsing.Token;
using JetBrains.Application.Threading;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Util;

namespace GammaJul.ForTea.Core.Parsing.Parser
{
	public class T4MissingTokenInserter : MissingTokenInserterBase
	{
		private readonly ILexer myLexer;

		private T4MissingTokenInserter(
			ILexer lexer,
			ITokenOffsetProvider offsetProvider,
			SeldomInterruptChecker interruptChecker,
			ITokenIntern intern
		) : base(offsetProvider, interruptChecker, intern) => myLexer = lexer;

		protected override void ProcessLeafElement(TreeElement leafElement)
		{
			int elemOffset = GetLeafOffset(leafElement).Offset;
			if (myLexer.TokenType != null && myLexer.TokenStart < elemOffset)
			{
				var anchor = leafElement;
				var parent = anchor.parent;
				while (anchor == parent.firstChild && parent.parent != null)
				{
					anchor = parent;
					parent = parent.parent;
				}

				while (myLexer.TokenType != null && myLexer.TokenStart < elemOffset)
				{
					var newToken = (TreeElement) TreeElementFactory.CreateLeafElement(myLexer);
					parent.AddChildBefore(newToken, anchor);
					myLexer.Advance();
				}
			}

			int skipTo = elemOffset + leafElement.GetTextLength();
			while (myLexer.TokenType != null && myLexer.TokenStart < skipTo)
			{
				myLexer.Advance();
			}
		}

		public static void Run(TreeElement root, ILexer lexer, ITokenOffsetProvider offsetProvider, ITokenIntern intern)
		{
			if (!(root is CompositeElement cRoot)) return;
			var eofNode = new T4TokenNodeType("", 0, null, T4TokenNodeFlag.None)
				.Create(lexer.Buffer, new TreeOffset(lexer.Buffer.Length), new TreeOffset(lexer.Buffer.Length));
			cRoot.AppendNewChild(eofNode);
			var inserter = new T4MissingTokenInserter(lexer, offsetProvider, new SeldomInterruptChecker(), intern);
			lexer.Start();
			inserter.Run(root);
			cRoot.DeleteChildRange(eofNode, eofNode);
		}
	}
}
