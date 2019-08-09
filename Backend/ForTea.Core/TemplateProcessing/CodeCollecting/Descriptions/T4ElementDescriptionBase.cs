namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Descriptions
{
	public abstract class T4ElementDescriptionBase
	{
		public bool IsVisible { get; private set; } = true;
		public void MakeInvisible() => IsVisible = false;
	}
}
