using System;
using System.Linq;
using GammaJul.ForTea.Core.Daemon.Highlightings;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration.Converters;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.Util;

namespace GammaJul.ForTea.Core.Daemon.Processes {
	
	public sealed class T4CSharpErrorProcess : CSharpIncrementalDaemonStageProcessBase {

		public override void VisitClassDeclaration(IClassDeclaration classDeclarationParam, IHighlightingConsumer context) {
			base.VisitClassDeclaration(classDeclarationParam, context);

			if (!classDeclarationParam.IsSynthetic()) return;
			if (!T4CSharpIntermediateConverterBase.GeneratedClassNameString.Equals(
				classDeclarationParam.DeclaredName, StringComparison.Ordinal))
				return;

			ITypeUsage baseClassNode = classDeclarationParam.SuperTypeUsageNodes.FirstOrDefault();
			if (baseClassNode?.IsVisibleInDocument() != true) return;

			if (T4CSharpIntermediateConverterBase.GeneratedBaseClassNameString.Equals(
				baseClassNode.GetText(),
				StringComparison.Ordinal)) return;

			ITypeElement baseClass = classDeclarationParam.SuperTypes.FirstOrDefault()?.GetTypeElement();
			if (baseClass == null) return;

			if (HasTransformTextMethod(baseClass)) return;
			context.AddHighlighting(new MissingTransformTextMethodError(baseClassNode, baseClass));
		}

		private static bool HasTransformTextMethod([NotNull] ITypeElement typeElement)
			=> typeElement
				.GetAllClassMembers(T4CSharpIntermediateConverterBase.DefaultTransformTextMethodName)
				.SelectNotNull(instance => instance.Member as IMethod)
				.Any(IsTransformTextMethod);

		private static bool IsTransformTextMethod([NotNull] IMethod method)
			=> method.ShortName == T4CSharpIntermediateConverterBase.DefaultTransformTextMethodName
			&& (method.IsVirtual || method.IsOverride || method.IsAbstract)
			&& !method.IsSealed
			&& method.GetAccessRights() == AccessRights.PUBLIC
			&& method.ReturnType.IsString()
			&& method.Parameters.Count == 0;

		public T4CSharpErrorProcess([NotNull] IDaemonProcess process, [NotNull] IContextBoundSettingsStore settingsStore, [NotNull] ICSharpFile file)
			: base(process, settingsStore, file) {
		}

	}

}
