using System;
using System.Collections.Generic;
using GammaJul.ForTea.Core.Psi.Directives.Attributes;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Descriptions;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.Interrupt;
using GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting.State;
using GammaJul.ForTea.Core.TemplateProcessing.CodeGeneration;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;

namespace GammaJul.ForTea.Core.TemplateProcessing.CodeCollecting
{
	public sealed class T4CSharpCodeGenerationIntermediateResult
	{
		[NotNull]
		public T4CSharpCodeGenerationResult CollectedBaseClass { get; }

		[CanBeNull]
		public string Encoding { get; set; }

		[NotNull, ItemNotNull]
		private List<IT4AppendableElementDescription> MyTransformationDescriptions { get; }

		[NotNull, ItemNotNull]
		private List<IT4AppendableElementDescription> MyFeatureDescriptions { get; }

		[NotNull, ItemNotNull]
		private List<T4ParameterDescription> MyParameterDescriptions { get; }

		[NotNull, ItemNotNull]
		private List<T4ImportDescription> MyImportDescriptions { get; }

		[NotNull, ItemNotNull]
		public IReadOnlyList<T4ParameterDescription> ParameterDescriptions => MyParameterDescriptions;

		[NotNull, ItemNotNull]
		public IReadOnlyList<T4ImportDescription> ImportDescriptions => MyImportDescriptions;

		[NotNull, ItemNotNull]
		public IReadOnlyList<IT4AppendableElementDescription> FeatureDescriptions => MyFeatureDescriptions;

		[NotNull, ItemNotNull]
		public IReadOnlyList<IT4AppendableElementDescription> TransformationDescriptions =>
			MyTransformationDescriptions;

		public IT4InfoCollectorState State { get; private set; }
		public bool FeatureStarted => State.FeatureStarted;
		public bool HasHost { get; private set; }
		private AccessRights AccessRights { get; set; }

		[NotNull]
		public string AccessRightsText
		{
			get => AccessRights switch
			{
				AccessRights.INTERNAL => T4VisibilityDirectiveAttributeInfo.Internal,
				_ => T4VisibilityDirectiveAttributeInfo.Public
			};
			set
			{
				if (T4VisibilityDirectiveAttributeInfo.Internal.Equals(value, StringComparison.OrdinalIgnoreCase))
					AccessRights = AccessRights.INTERNAL;
				else
					AccessRights = AccessRights.PUBLIC;
			}
		}

		public void AdvanceState([NotNull] ITreeNode element) => State = State.GetNextState(element);
		public void RequireHost() => HasHost = true;
		public bool HasBaseClass => !CollectedBaseClass.IsEmpty;

		public T4CSharpCodeGenerationIntermediateResult(
			[NotNull] IT4File file,
			[NotNull] IT4CodeGenerationInterrupter interrupter
		)
		{
			CollectedBaseClass = new T4CSharpCodeGenerationResult(file);
			MyTransformationDescriptions = new List<IT4AppendableElementDescription>();
			MyFeatureDescriptions = new List<IT4AppendableElementDescription>();
			MyParameterDescriptions = new List<T4ParameterDescription>();
			MyImportDescriptions = new List<T4ImportDescription>();
			State = new T4InfoCollectorStateInitial(interrupter);
			HasHost = false;
		}

		public void Append([NotNull] T4ParameterDescription description) => MyParameterDescriptions.Add(description);
		public void Append([NotNull] T4ImportDescription description) => MyImportDescriptions.Add(description);

		public void AppendFeature([NotNull] IT4AppendableElementDescription description) =>
			MyFeatureDescriptions.Add(description);

		public void AppendFeature([NotNull] string message) =>
			MyFeatureDescriptions.Add(new T4TextDescription(message));

		public void AppendTransformation([NotNull] IT4AppendableElementDescription description) =>
			MyTransformationDescriptions.Add(description);

		public void AppendTransformation([NotNull] string message) =>
			MyTransformationDescriptions.Add(new T4TextDescription(message));

		public void Append([NotNull] T4CSharpCodeGenerationIntermediateResult other)
		{
			MyImportDescriptions.AddRange(other.ImportDescriptions);
			Encoding = Encoding ?? other.Encoding;
			if (CollectedBaseClass.IsEmpty) CollectedBaseClass.Append(other.CollectedBaseClass);
			MyTransformationDescriptions.AddRange(other.TransformationDescriptions);
			MyFeatureDescriptions.AddRange(other.FeatureDescriptions);
			MyParameterDescriptions.AddRange(other.ParameterDescriptions);
			// 'feature started' is intentionally ignored
			HasHost = HasHost || other.HasHost;
		}
	}
}
