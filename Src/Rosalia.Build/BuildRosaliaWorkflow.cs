namespace Rosalia.Build
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Yaml.Serialization;
    using Rosalia.Build.Helpers;
    using Rosalia.Build.Tasks;
    using Rosalia.Core;
    using Rosalia.Core.Context;
    using Rosalia.Core.FileSystem;
    using Rosalia.Core.Tasks.Flow;
    using Rosalia.TaskLib.AssemblyInfo;
    using Rosalia.TaskLib.Git.Input;
    using Rosalia.TaskLib.Git.Tasks;
    using Rosalia.TaskLib.MsBuild;
    using Rosalia.TaskLib.NuGet.Tasks;

    public class BuildRosaliaWorkflow : Workflow<BuildRosaliaContext>
    {
        public override ITask<BuildRosaliaContext> RootTask
        {
            get
            {
                return Sequence(
                    //// Initialize context
                    Task((builder, context) =>
                    {
                        var data = context.Data;

                        data.Configuration = "Debug";
                        data.ProjectRootDirectory = context.WorkDirectory.Parent.Parent;
                        data.Src = data.ProjectRootDirectory.GetDirectory("Src");
                        data.SolutionFile = data.Src.GetFile("Rosalia.sln");
                        data.Artifacts = data.ProjectRootDirectory.GetDirectory("Artifacts");          
                    }),
                    //// Get current version from git
                    new GetVersionTask<BuildRosaliaContext>(
                        (output, c) =>
                        {
                            c.Version = output.Tag.Replace("v", string.Empty) + "." + output.CommitsCount;
                        }),
                    //// Generate common assembly info file
                    new GenerateAssemblyInfo<BuildRosaliaContext>()
                        .WithAttribute(c => new AssemblyProductAttribute("Rosalia"))
                        .WithAttribute(c => new AssemblyVersionAttribute(c.Version))
                        .WithAttribute(c => new AssemblyFileVersionAttribute(c.Version))
                        .ToFile(c => c.Data.Src.GetFile("CommonAssemblyInfo.cs")),
                    //// Build solution
                    new MsBuildTask<BuildRosaliaContext>()
                        .FillInput(c => new MsBuildInput()
                            .WithProjectFile(c.Data.SolutionFile)
                            .WithConfiguration(c.Data.Configuration)),
                    //// Generate spec for Core NuGet package
                    new GenerateNuGetSpecTask<BuildRosaliaContext>((c, input) =>
                        input
                            .Id("Rosalia.Core")
                            .FillCommonProperties(c)
                            .Description("Core libs for Rosalia framework.")
                            .WithFiles(GetCoreLibFiles(c), "lib")
                            .ToFile(c.Data.NuSpecRosaliaCore)),
                    //// Generate specs for NuGet packages
                    new GenerateNuGetSpecTask<BuildRosaliaContext>((c, input) =>
                        input
                            .Id("Rosalia")
                            .FillCommonProperties(c)
                            .Description("Automation tool that allows writing build, install or other workflows in C#. This package automatically integrates Rosalia to your Class Library project.")
                            .WithFile(c.Data.RosaliaPackageInstallScript, "tools")
                            .WithFile(c.Data.RosaliaRunnerConsoleExe, "tools")
                            .WithFiles(GetRunnerDllFiles(c), "tools")
                            .WithFiles(c.Data.BuildAssets.Files.Include(fileName => fileName.EndsWith(".pp")), "content")
                            .WithDependency("Rosalia.Core", c.Data.Version)
                            .WithDependency("NuGetPowerTools", version: "0.29")
                            .ToFile(c.Data.NuSpecRosalia)),
                    //// Generate spec for Task Libs
                    ForEach(c => c.FindTaskLibDirectories())
                        .Do((context, directory) =>
                            new GenerateNuGetSpecTask<BuildRosaliaContext>((taskContext, input) => input
                                .FillCommonProperties(taskContext)
                                .FillTaskLibProperties(taskContext, directory.Name.Replace("Rosalia.TaskLib.", string.Empty)))),
                    //// Generate NuGet packages
                    ForEach(c => c.Data.Artifacts.Files.Include(fileName => fileName.EndsWith(".nuspec")))
                        .Do((context, file) => new GeneratePackageTask<BuildRosaliaContext>(file)),
                    //// 
                    //// Genarate docs
                    //// 
                    new BuildDocsTask(),
                    //// Push documentation to GhPages
                    If(c => c.WorkDirectory.GetFile("private_data.yaml").Exists).Then(PrepareDocsForDeployment));
            }
        }

        private SequenceTask<BuildRosaliaContext> PrepareDocsForDeployment
        {
            get
            {
                return Sequence(
                    //// Read private data
                    Task((builder, c) =>
                    {
                        var serializer = new YamlSerializer();
                        c.Data.PrivateData = (PrivateData)serializer.DeserializeFromFile(c.WorkDirectory.GetFile("private_data.yaml").AbsolutePath, typeof(PrivateData))[0];
                    }),
                    //// Copy docs artifats to GhPages repo
                    Task((builder, context) =>
                    {
                        var docsHost = context.Data.Src.GetDirectory("Rosalia.Docs");
                        var files = docsHost
                            .SearchFilesIn()
                            .IncludeByRelativePath(relative => relative.Equals("index.html") || relative.StartsWith("content"));

                        files.CopyRelativelyTo(new DefaultDirectory(context.Data.PrivateData.GhPagesRoot));
                    }),
                    //// Add all doc files to gh-pages repo
                    new GitCommandTask<BuildRosaliaContext>
                    {
                        InputProvider = context => new GitInput
                        {
                            RawCommand = "add .",
                            WorkDirectory = new DefaultDirectory(context.Data.PrivateData.GhPagesRoot)
                        }
                    },
                    //// Do auto commit to gh-pages repo
                    new GitCommandTask<BuildRosaliaContext>
                    {
                        InputProvider = context => new GitInput
                        {
                            RawCommand = string.Format("commit -a -m \"Docs auto commit v{0}\"", context.Data.Version),
                            WorkDirectory = new DefaultDirectory(context.Data.PrivateData.GhPagesRoot)
                        }
                    });
            }
        }

        private static IEnumerable<IFile> GetCoreLibFiles(TaskContext<BuildRosaliaContext> c)
        {
            return c.FileSystem.SearchFilesIn(c.Data.RosaliaRunnerConsoleBin)
                    .Include(file => file.Name == "Rosalia.Core.dll" || file.Name == "Rosalia.TaskLib.Standard.dll");
        }

        private static IEnumerable<IFile> GetRunnerDllFiles(TaskContext<BuildRosaliaContext> c)
        {
            return c.FileSystem.SearchFilesIn(c.Data.RosaliaRunnerConsoleBin)
                .IncludeByFileName(
                    "Rosalia.exe",
                    "Rosalia.Core.dll",
                    "Rosalia.Core.Watchers.dll",
                    "Rosalia.Runner.dll",
                    "Rosalia.TaskLib.Standard.dll",
                    "Rosalia.TaskLib.MsBuild.dll");
        }
    }
}