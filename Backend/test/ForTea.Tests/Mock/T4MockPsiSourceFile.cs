using System;
using System.Collections.Generic;
using JetBrains.DocumentModel;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.Util;

namespace JetBrains.ForTea.Tests.Mock
{
	public sealed class T4MockPsiSourceFile : IPsiSourceFile, IPsiSourceFileWithLocation
	{
		public T GetData<T>(Key<T> key) where T : class => throw new NotImplementedException();
		public void PutData<T>(Key<T> key, T value) where T : class => throw new NotImplementedException();
		public T GetOrCreateDataUnderLock<T>(Key<T> key, Func<T> factory) where T : class => throw new NotImplementedException();
		public T GetOrCreateDataUnderLock<T, TState>(Key<T> key, TState state, Func<TState, T> factory) where T : class => throw new NotImplementedException();
		public IEnumerable<KeyValuePair<object, object>> EnumerateData() => throw new NotImplementedException();
		public string GetPersistentID() => throw new NotImplementedException();
		public bool IsValid() => throw new NotImplementedException();
		public IPsiModule PsiModule => throw new NotImplementedException();
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
	}
}
