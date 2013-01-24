namespace Rosalia.Build
{
    using System.Collections.Generic;
    using System.Reflection;
    using Rosalia.Core;
    using Rosalia.Core.Context;
    using Rosalia.Core.FileSystem;
    using Rosalia.TaskLib.AssemblyInfo;
    using Rosalia.TaskLib.Git;
    using Rosalia.TaskLib.MsBuild;
    using Rosalia.TaskLib.NuGet;

    public class BuildRosaliaWorkflow : Workflow<BuildRosaliaContext>
    {
        public override ITask<BuildRosaliaContext> RootTask
        {
            get
            {
                return Sequence(
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
                            .WithProjectFile(c.Data.SolutionFile)),
                    //// Generate spec for Core NuGet package
                    new GenerateNuGetSpecTask<BuildRosaliaContext>((c, input) => 
                        input
                            .Id("Rosalia.Core")
                            .Version(c.Data.Version)
                            .Authors("Eugene Guryanov")
                            .Description("Core libs for Rosalia framework")
                            .WithFiles(GetCoreLibFiles(c), "lib")
                            .ToFile(c.Data.NuSpecRosaliaCore)),
                    //// Generate specs for NuGet packages
                    new GenerateNuGetSpecTask<BuildRosaliaContext>((c, input) => 
                        input
                            .Id("Rosalia")
                            .Version(c.Data.Version)
                            .Authors("Eugene Guryanov")
                            .Description("Simple workflow execution framework/tool that could be used for build scripts")
                            .WithFile(c.Data.RosaliaPackageInstallScript, "tools")
                            .WithFile(c.Data.RosaliaRunnerConsoleExe, "tools")
                            .WithFiles(GetRunnerDllFiles(c), "tools")
                            .WithFiles(c.Data.BuildAssets.Files.Include(fileName => fileName.EndsWith(".pp")), "content")
                            .WithDependency("Rosalia.Core", version: c.Data.Version)
                            .WithDependency("NuGetPowerTools", version: "0.29")
                            .ToFile(c.Data.NuSpecRosalia)),
                    //// Generate NuGet packages
                    new GeneratePackageTask<BuildRosaliaContext>(c => c.Data.NuSpecRosaliaCore),
                    new GeneratePackageTask<BuildRosaliaContext>(c => c.Data.NuSpecRosalia));
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
                    .Include(fileName => fileName.EndsWith(".dll"))
                    .Exclude(
                        file => file.Name.Contains(".TaskLib.") && 
                            !(file.Name == "Rosalia.TaskLib.Standard" || 
                              file.Name == "Rosalia.TaskLib.MsBuild"));
        }

        protected override void OnBeforeExecute(TaskContext<BuildRosaliaContext> context)
        {
            base.OnBeforeExecute(context);

            var data = context.Data;

            data.Configuration = "Debug";
            data.ProjectRootDirectory = context.WorkDirectory.Parent.Parent;
            data.Src = data.ProjectRootDirectory.GetDirectory("Src");
            data.SolutionFile = data.Src.GetFile("Rosalia.sln");
            data.Artifacts = data.ProjectRootDirectory.GetDirectory("Artifacts");
        }
    }
}