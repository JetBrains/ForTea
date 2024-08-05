using System;
using System.Linq;
using GammaJul.ForTea.Core.Psi;
using GammaJul.ForTea.Core.Tree;
using JetBrains.Annotations;
using JetBrains.Application.Parts;
using JetBrains.Core;
using JetBrains.ForTea.RiderPlugin.Model;
using JetBrains.ProjectModel;
using JetBrains.RdBackend.Common.Features.ProjectModel.View;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.Util;

namespace JetBrains.ForTea.RiderPlugin.ProtocolAware.Impl
{
  [SolutionComponent(InstantiationEx.LegacyDefault)]
  public sealed class T4ModelInteractionHelper : IT4ModelInteractionHelper
  {
    [NotNull] private ProjectModelViewHost Host { get; }

    [NotNull] private ILogger Logger { get; }

    public T4ModelInteractionHelper([NotNull] ProjectModelViewHost host, [NotNull] ILogger logger)
    {
      Host = host;
      Logger = logger;
    }

    public Func<T4FileLocation, T> Wrap<T>(Func<IT4File, T> wrappee, T defaultValue)
      where T : class =>
      Wrap(sourceFile => wrappee(sourceFile
        .GetPsiFiles(T4Language.Instance)
        .OfType<IT4File>()
        .SingleItem()), defaultValue);

    public Func<T4FileLocation, T> Wrap<T>(Func<IPsiSourceFile, T> wrappee, T defaultValue)
      where T : class =>
      location =>
      {
        var result = Logger.Catch(() =>
        {
          using (ReadLockCookie.Create())
          {
            var file = Host.GetItemById<IProjectFile>(location.Id)?.ToSourceFile();
            return file == null ? null : wrappee(file);
          }
        });
        return result ?? defaultValue;
      };

    public Func<T4FileLocation, Unit> Wrap(Action<IT4File> wrappee) => location =>
    {
      Logger.Catch(() =>
      {
        using (ReadLockCookie.Create())
        {
          var projectFile = Host.GetItemById<IProjectFile>(location.Id);
          var sourceFile = projectFile?.ToSourceFile();
          if (sourceFile == null) return;
          sourceFile.GetPsiServices().Files.CommitAllDocuments();
          var file = sourceFile.GetPsiFiles(T4Language.Instance).OfType<IT4File>().SingleItem();
          if (file != null) wrappee(file);
        }
      });
      return Unit.Instance;
    };
  }
}