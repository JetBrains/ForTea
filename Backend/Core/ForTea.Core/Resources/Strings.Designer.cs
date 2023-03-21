namespace GammaJul.ForTea.Core.Resources
{
  using System;
  using JetBrains.Application.I18n;
  using JetBrains.DataFlow;
  using JetBrains.Diagnostics;
  using JetBrains.Lifetimes;
  using JetBrains.Util;
  using JetBrains.Util.Logging;
  
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
  [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
  public static class Strings
  {
    private static readonly ILogger ourLog = Logger.GetLogger("GammaJul.ForTea.Core.Resources.Strings");

    static Strings()
    {
      CultureContextComponent.Instance.WhenNotNull(Lifetime.Eternal, (lifetime, instance) =>
      {
        lifetime.Bracket(() =>
          {
            ourResourceManager = new Lazy<JetResourceManager>(
              () =>
              {
                return instance
                  .CreateResourceManager("GammaJul.ForTea.Core.Resources.Strings", typeof(Strings).Assembly);
              });
          },
          () =>
          {
            ourResourceManager = null;
          });
      });
    }
    
    private static Lazy<JetResourceManager> ourResourceManager = null;
    
    [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
    public static JetResourceManager ResourceManager
    {
      get
      {
        var resourceManager = ourResourceManager;
        if (resourceManager == null)
        {
          return ErrorJetResourceManager.Instance;
        }
        return resourceManager.Value;
      }
    }

    public static string T4HighlighterMacro_Text => ResourceManager.GetString("T4HighlighterMacro_Text");
    public static string T4HighlighterEnvironmentVariable_Text => ResourceManager.GetString("T4HighlighterEnvironmentVariable_Text");
    public static string T4HighlighterAttributeValue_Text => ResourceManager.GetString("T4HighlighterAttributeValue_Text");
    public static string T4HihglighterDirectiveName_Text => ResourceManager.GetString("T4HihglighterDirectiveName_Text");
    public static string T4highlighterAttributeName_Text => ResourceManager.GetString("T4highlighterAttributeName_Text");
  }
}