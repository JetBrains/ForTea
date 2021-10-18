using System.Collections.Generic;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Descriptions;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters.ClassName;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters.GeneratorKind;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters
{
	public sealed class T4CSharpExecutableCodeBehindIntermediateConverter : T4CSharpCodeBehindIntermediateConverter
	{
		public T4CSharpExecutableCodeBehindIntermediateConverter([NotNull] IT4File file) : base(
			file,
			new T4ExecutableClassNameProvider(),
			new T4ExecutableGeneratorKind()
		)
		{
		}

		// we reference JetBrains.TextTemplating, which already contains the definition for TextTransformation
		protected override void AppendClasses(T4CSharpCodeGenerationIntermediateResult intermediateResult) =>
			AppendClass(intermediateResult);

		protected override string GetTransformTextOverridabilityModifier(bool hasCustomBaseClass) => OverrideKeyword;

		#region SyntheticAttribute
		private void AppendSyntheticAttribute()
		{
			AppendIndent();
			Result.AppendLine($"[{SyntheticAttribute.Name}]");
		}

		protected override void AppendFieldDeclaration(T4ParameterDescription description)
		{
			AppendSyntheticAttribute();
			base.AppendFieldDeclaration(description);
		}

		protected override void AppendClass(T4CSharpCodeGenerationIntermediateResult intermediateResult)
		{
			AppendSyntheticAttribute();
			base.AppendClass(intermediateResult);
		}

		protected override void AppendTransformMethod(T4CSharpCodeGenerationIntermediateResult intermediateResult)
		{
			AppendSyntheticAttribute();
			base.AppendTransformMethod(intermediateResult);
		}

		protected override void AppendTemplateInitialization(
			IReadOnlyCollection<T4ParameterDescription> descriptions,
			bool hasHost
		)
		{
			if (descriptions.IsEmpty()) return;
			AppendSyntheticAttribute();
			base.AppendTemplateInitialization(descriptions, hasHost);
		}
		#endregion
	}
}
