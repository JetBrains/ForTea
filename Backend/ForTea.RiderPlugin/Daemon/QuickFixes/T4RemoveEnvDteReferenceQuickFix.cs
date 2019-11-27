using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.Daemon.QuickFixes.Removing;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Diagnostics;
using JetBrains.ReSharper.Feature.Services.QuickFixes;

namespace JetBrains.ForTea.RiderPlugin.Daemon.QuickFixes
{
	[QuickFix]
	public class T4RemoveEnvDteReferenceQuickFix :
		T4RemoveBlockQuickFixBase<IT4AssemblyDirective, NoSupportForEnvDteError>
	{
		public T4RemoveEnvDteReferenceQuickFix([NotNull] NoSupportForEnvDteError highlighting) : base(highlighting)
		{
		}

		protected override IT4AssemblyDirective Node
		{
			get
			{
				var value = Highlighting.Value;
				var attribute = DirectiveAttributeNavigator.GetByValue(value);
				return AssemblyDirectiveNavigator.GetByAttribute(attribute).NotNull();
			}
		}
	}
}
