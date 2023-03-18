using GammaJul.ForTea.Core.Psi.FileType;
using JetBrains.Application.Components;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.FileTypes;
using JetBrains.ReSharper.Feature.Services.CodeStructure;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;

namespace JetBrains.ForTea.Tests.Features
{
  [TestFileExtension(T4FileExtensions.MainExtension)]
  public sealed class T4CodeStructureTest : BaseTestWithSingleProject
  {
    protected override string RelativeTestDataPath => @"Features\CodeStructure";

    [Test]
    public void TestBasicTemplate() => DoNamedTest2();

    [Test]
    public void TestAttributeWithoutValue() => DoNamedTest2();

    [Test]
    public void TestEmptyFile() => DoNamedTest2();

    [Test]
    public void TestDirectiveWithoutAttributes() => DoNamedTest2();

    [Test]
    public void TestDirectiveWithUnknownAttributes() => DoNamedTest2();

    [Test]
    public void TestIncompleteDirective() => DoNamedTest2();

    [Test]
    public void TestDuplicateDirective() => DoNamedTest2();

    [Test]
    public void TestDuplicateAttribute() => DoNamedTest2();

    [Test]
    public void TestUnresolvedInclude() => DoNamedTest2();

    [Test]
    public void TestAllShownAttributes() => DoNamedTest2();

    [Test]
    public void TestFeatureBlocks() => DoNamedTest2();

    #region For test running

    private string myTestName;

    protected override void DoOneTest(string testName)
    {
      myTestName = testName + Extension;
      DoTestSolution(myTestName);
    }

    protected override void DoTest(Lifetime lifetime, IProject testProject)
    {
      var projectFile = testProject.GetSubItems(myTestName).Single() as IProjectFile;
      Assert.IsNotNull(projectFile, "projectFile != null");

      var sourceFile = projectFile.ToSourceFile();
      Assert.IsNotNull(sourceFile, "sourceFile != null");

      var builder = ShellInstance
        .GetComponent<IProjectFileTypeServices>()
        .GetService<IProjectFileCodeStructureProvider>(T4ProjectFileType.Instance);
      var rootElement = builder.Build(sourceFile, new CodeStructureOptions { BuildInheritanceInformation = true });
      ExecuteWithGold(projectFile, sb => rootElement.Dump(sb));
    }

    #endregion
  }
}