using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Application.Threading;
using JetBrains.DocumentModel;
using JetBrains.Lifetimes;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Properties;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.Util;
using JetBrains.Util.Dotnet.TargetFrameworkIds;

namespace JetBrains.ForTea.Tests.Mock
{
	public sealed class T4MockPsiSourceFile : IPsiProjectFile, IPsiSourceFileWithLocation
	{
		public bool IsValid() => true;
		public IPsiModule PsiModule => new T4MockPsiModule();

		public T GetData<T>(Key<T> key) where T : class => throw new NotImplementedException();
		public void PutData<T>(Key<T> key, T value) where T : class => throw new NotImplementedException();
		public T GetOrCreateDataUnderLock<T>(Key<T> key, Func<T> factory) where T : class => throw new NotImplementedException();
		public T GetOrCreateDataUnderLock<T, TState>(Key<T> key, TState state, Func<TState, T> factory) where T : class => throw new NotImplementedException();
		public IEnumerable<KeyValuePair<object, object>> EnumerateData() => throw new NotImplementedException();
		public string GetPersistentID() => throw new NotImplementedException();
		public IDocument Document => throw new NotImplementedException();
		public string Name => throw new NotImplementedException();
		public string DisplayName => throw new NotImplementedException();
		public ProjectFileType LanguageType => throw new NotImplementedException();
		public PsiLanguageType PrimaryPsiLanguage => throw new NotImplementedException();
		public IPsiSourceFileProperties Properties => throw new NotImplementedException();
		public IModuleReferenceResolveContext ResolveContext => throw new NotImplementedException();
		public IPsiSourceFileStorage PsiStorage => throw new NotImplementedException();
		public int? InMemoryModificationStamp => throw new NotImplementedException();
		public int? ExternalModificationStamp => throw new NotImplementedException();
		public DateTime LastWriteTimeUtc => throw new NotImplementedException();
		public FileSystemPath Location => FileSystemPath.Parse("C:\\dummy.tt");
		public IProjectFile ProjectFile => new T4MockProjectFile();
	}

	public sealed class T4MockPsiModule : IPsiModule
	{
		public T GetData<T>(Key<T> key) where T : class => throw new NotImplementedException();
		public void PutData<T>(Key<T> key, T value) where T : class => throw new NotImplementedException();
		public T GetOrCreateDataUnderLock<T>(Key<T> key, Func<T> factory) where T : class => throw new NotImplementedException();
		public T GetOrCreateDataUnderLock<T, TState>(Key<T> key, TState state, Func<TState, T> factory) where T : class => throw new NotImplementedException();
		public IEnumerable<KeyValuePair<object, object>> EnumerateData() => throw new NotImplementedException();
		public IPsiServices GetPsiServices() => throw new NotImplementedException();
		public ISolution GetSolution() => throw new NotImplementedException();
		public IEnumerable<IPsiModuleReference> GetReferences(IModuleReferenceResolveContext moduleReferenceResolveContext) => throw new NotImplementedException();
		public ICollection<PreProcessingDirective> GetAllDefines() => throw new NotImplementedException();
		public bool IsValid() => throw new NotImplementedException();
		public string GetPersistentID() => throw new NotImplementedException();
		public string Name => throw new NotImplementedException();
		public string DisplayName => throw new NotImplementedException();
		public TargetFrameworkId TargetFrameworkId => throw new NotImplementedException();
		public PsiLanguageType PsiLanguage => throw new NotImplementedException();
		public ProjectFileType ProjectFileType => throw new NotImplementedException();
		public IModule ContainingProjectModule => throw new NotImplementedException();
		public IEnumerable<IPsiSourceFile> SourceFiles => throw new NotImplementedException();
	}

	public sealed class T4MockProjectFile : IProjectFile
	{
		public T GetData<T>(Key<T> key) where T : class => throw new NotImplementedException();
		public void PutData<T>(Key<T> key, T value) where T : class => throw new NotImplementedException();
		public T GetOrCreateDataUnderLock<T>(Key<T> key, Func<T> factory) where T : class => throw new NotImplementedException();
		public T GetOrCreateDataUnderLock<T, TState>(Key<T> key, TState state, Func<TState, T> factory) where T : class => throw new NotImplementedException();
		public IEnumerable<KeyValuePair<object, object>> EnumerateData() => throw new NotImplementedException();
		public void Accept(ProjectVisitor projectVisitor) => throw new NotImplementedException();
		public ISolution GetSolution() => throw new NotImplementedException();
		public object GetProperty(Key propertyName) => throw new NotImplementedException();
		public void SetProperty(Key propertyName, object propertyValue) => throw new NotImplementedException();
		public bool IsValid() => throw new NotImplementedException();
		public string Name => throw new NotImplementedException();
		public Type MarshallerType => throw new NotImplementedException();
		public IProject GetProject() => throw new NotImplementedException();
		public string GetPersistentID() => throw new NotImplementedException();
		public void Dump(TextWriter to, DumpFlags flags = DumpFlags.DEFAULT) => throw new NotImplementedException();
		public string GetPresentableProjectPath() => throw new NotImplementedException();
		public IProjectFolder ParentFolder => throw new NotImplementedException();
		public FileSystemPath Location => throw new NotImplementedException();
		public ProjectItemKind Kind => throw new NotImplementedException();
		public bool IsLinked => throw new NotImplementedException();
		public IShellLocks Locks => throw new NotImplementedException();
		public void MarkReconciledWithInMemoryVersion() => throw new NotImplementedException();
		public void MarkReconciledWithExternalVersion() => throw new NotImplementedException();
		public Stream CreateReadStream(Lifetime lifetime) => throw new NotImplementedException();
		public Stream CreateWriteStream(Lifetime lifetime) => throw new NotImplementedException();
		public IProjectFile GetDependsUponFile() => throw new NotImplementedException();
		public ICollection<IProjectFile> GetDependentFiles() => throw new NotImplementedException();
		public void UpdatePropertiesFrom(IProjectFileProperties properties) => throw new NotImplementedException();
		public ProjectFileType LanguageType => throw new NotImplementedException();
		public int LastExternalModificationStamp => throw new NotImplementedException();
		public DateTime LastWriteTimeUtc => throw new NotImplementedException();
		public bool IsMissing => throw new NotImplementedException();
		public bool HasChangedExternallySinceLastReconciliation => throw new NotImplementedException();
		public bool HasChangedInMemorySinceLastReconciliation => throw new NotImplementedException();
		public bool IsFileSystemReadonly => throw new NotImplementedException();
		public IProjectFileProperties Properties => throw new NotImplementedException();
		public int LastInMemoryModificationStamp { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public IProjectElementOrigin Origin { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
	}
}
