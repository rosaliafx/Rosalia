namespace Rosalia.TaskLib.NuGet.Tests
{
    using NUnit.Framework;
    using Rosalia.Core.Tests;
    using Rosalia.Core.Tests.Stubs;

    public class GenerateNuGetSpecTaskTests : TaskTestsBase<object>
    {
        [Test]
        public void Execute_WithMetadata_ShouldRenderMetadata()
        {
            var destination = new FileStub();

            var task = new GenerateNuGetSpecTask<object>()
                .FillInput(c => new SpecInput()
                    .Id("testId")
                    .ToFile(destination));

            Execute(task);

            Assert.That(destination.Content, Is.StringContaining(
@"<?xml version='1.0' encoding='utf-8'?>
<package xmlns='http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd'>
  <metadata>
    <id>testId</id>
  </metadata>
</package>"));
        }

        [Test]
        public void Execute_WithDependencies_ShouldRenderMetadata()
        {
            var destination = new FileStub();

            var task = new GenerateNuGetSpecTask<object>()
                .FillInput(c => new SpecInput()
                    .WithDependency("id1", "v1.0")
                    .WithDependency("id2")
                    .WithDependency("id3", "v1.0", "net40")
                    .ToFile(destination));

            Execute(task);

            Assert.That(destination.Content.Trim(), Is.EqualTo(
@"<?xml version='1.0' encoding='utf-8'?>
<package xmlns='http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd'>
  <metadata>
    <dependencies>
      <group>
        <dependency id='id1' version='v1.0' />
        <dependency id='id2' />
      </group>
      <group targetFramework='net40'>
        <dependency id='id3' version='v1.0' />
      </group>
    </dependencies>
  </metadata>
</package>"));
        }

        [Test]
        public void Execute_WithReferences_ShouldRenderReferences()
        {
            var destination = new FileStub();

            var task = new GenerateNuGetSpecTask<object>()
                .FillInput(c => new SpecInput()
                    .WithReference("My.Reference1.dll")
                    .WithReference("My.Reference2.dll")
                    .ToFile(destination));

            Execute(task);

            Assert.That(destination.Content.Trim(), Is.EqualTo(
@"<?xml version='1.0' encoding='utf-8'?>
<package xmlns='http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd'>
  <metadata>
    <references>
      <reference file='My.Reference1.dll' />
      <reference file='My.Reference2.dll' />
    </references>
  </metadata>
</package>"));
        }

        [Test]
        public void Execute_WithFrameworkAssemblies_ShouldRenderFrameworkAssemblies()
        {
            var destination = new FileStub();

            var task = new GenerateNuGetSpecTask<object>()
                .FillInput(c => new SpecInput()
                    .WithFrameworkAssembly("System.ServiceModel", "net40")
                    .WithFrameworkAssembly("System.SomethingElse")
                    .ToFile(destination));

            Execute(task);

            Assert.That(destination.Content.Trim(), Is.EqualTo(
@"<?xml version='1.0' encoding='utf-8'?>
<package xmlns='http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd'>
  <metadata>
    <frameworkAssemblies>
      <frameworkAssembly assemblyName='System.ServiceModel' targetFramework='net40' />
      <frameworkAssembly assemblyName='System.SomethingElse' />
    </frameworkAssemblies>
  </metadata>
</package>"));
        }

        [Test]
        public void Execute_WithFiles_ShouldRenderFiles()
        {
            var destination = new FileStub();

            var task = new GenerateNuGetSpecTask<object>()
                .FillInput(c => new SpecInput()
                    .WithFile(@"bin\Debug\*.dll", "lib")
                    .WithFile(@"bin\Debug\*.pdb", "lib")
                    .WithFile(@"tools\**\*.*", null, @"**\*.log")
                    .ToFile(destination));

            Execute(task);

            Assert.That(destination.Content.Trim(), Is.EqualTo(
@"<?xml version='1.0' encoding='utf-8'?>
<package xmlns='http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd'>
  <metadata>
  </metadata>
  <files>
    <file src='bin\Debug\*.dll' target='lib' />
    <file src='bin\Debug\*.pdb' target='lib' />
    <file src='tools\**\*.*' exclude='**\*.log' />
  </files>
</package>"));
        }
    }
}