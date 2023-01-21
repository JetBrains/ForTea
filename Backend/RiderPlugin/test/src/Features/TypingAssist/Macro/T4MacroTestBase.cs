using JetBrains.Annotations;
using JetBrains.ReSharper.FeaturesTestFramework.TypingAssist;
using JetBrains.ReSharper.TestFramework;
using JetBrains.TextControl;

namespace JetBrains.ForTea.Tests.Features.TypingAssist.Macro
{
  public abstract class T4MacroTestBase : TypingAssistTestBase
  {
    [NotNull] protected abstract string StringToType { get; }

    protected sealed override string RelativeTestDataPath => @"Features\TypingAssist\Macro";

    protected sealed override void DoAdditionalTyping(
      [NotNull] ITextControl textControl,
      TestOptionsIterator.TestData data
    )
    {
      foreach (char c in StringToType)
      {
        textControl.EmulateTyping(c);
      }
    }
  }
}