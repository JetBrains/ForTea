using System.Linq;
using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Properties;
using JetBrains.ReSharper.Host.Features.ProjectModel;

namespace GammaJul.ForTea.Core.TemplateProcessing.Services.Impl
{
	[SolutionComponent]
	public class T4ProjectModelTemplateDataManager : IT4ProjectModelTemplateDataManager
	{
		[NotNull]
		private ISolution Solution { get; }

		public T4ProjectModelTemplateDataManager([NotNull] ISolution solution) => Solution = solution;

		public T4TemplateKind GetTemplateKind(IProjectFile file)
		{
			var properties = file.Properties as ProjectFileProperties;
			string customTool = properties?.CustomTool;
			return T4TemplateManagerConstants.ToTemplateKind(customTool);
		}

		public void SetTemplateKind(IProjectFile file, T4TemplateKind kind) => Solution.InvokeUnderTransaction(cookie =>
			cookie.EditFileProperties(file, properties =>
				((ProjectFileProperties) properties).CustomTool = T4TemplateManagerConstants.ToRawValue(kind)
			)
		);

		public IProjectFile FindLastGenOutput(IProjectFile file)
		{
			var properties = file.Properties as ProjectFileProperties;
			string targetFileName = properties?.CustomToolOutput;
			if (targetFileName == null) return null;
			return file
				.ParentFolder
				?.GetSubItems(targetFileName)
				.AsEnumerable()
				.OfType<IProjectFile>()
				.SingleOrDefault(suspect => file == suspect.GetDependsUponFile());
		}

		public void SetLastGenOutput(IProjectFile source, IProjectFile generated) =>
			Solution.InvokeUnderTransaction(cookie =>
				cookie.EditFileProperties(source, properties =>
					((ProjectFileProperties) properties).CustomToolOutput = generated.Name
				)
			);
	}
}
