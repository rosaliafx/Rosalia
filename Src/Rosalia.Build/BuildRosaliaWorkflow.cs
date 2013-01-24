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
                return Sequence
                    //// Get current version from git
                    .WithSubtask(new GetVersionTask<BuildRosaliaContext>(
                        (output, c) =>
                        {
                            c.Version = output.Tag.Replace("v", string.Empty) + "." + output.CommitsCount;
                        }))

                    //// Generate common assembly info file
                    .WithSubtask(new GenerateAssemblyInfo<BuildRosaliaContext>()
                        .WithAttribute(c => new AssemblyProductAttribute("Rosalia"))
                        .WithAttribute(c => new AssemblyVersionAttribute(c.Version))
                        .ToFile(c => c.Data.Src.GetFile("CommonAssemblyInfo.cs")))

                    //// Build solution
                    .WithSubtask(new MsBuildTask<BuildRosaliaContext>()
                        .FillInput(c => new MsBuildInput()
                            .WithProjectFile(c.Data.SolutionFile)))

                    //// Generate specs for NuGet packages
                    .WithSubtask(GenerateNuGetSpec)

                    //// Generate NuGet packages
                    .WithSubtask(new GeneratePackageTask<BuildRosaliaContext>(GetConsoleRunnerSpecFile));
            }
        }

        private static ITask<BuildRosaliaContext> GenerateNuGetSpec
        {
            get
            {
                return new GenerateNuGetSpecTask<BuildRosaliaContext>()
                    .FillInput(c =>
                        new SpecInput()
                            .Id("Rosalia")
                            .Version(c.Data.Version)
                            .Authors("Eugene Guryanov")
                            .Description("Simple workflow execution framework/tool that could be used for build scripts")
                            .WithFiles(GetLibFiles(c), "lib")
                            .WithFile(c.Data.Src.GetFile(@"Rosalia.Build\Assets\Install.ps1").AbsolutePath, "tools")
                            .WithFile(string.Format(@"bin\{0}\Rosalia.exe", c.Data.Configuration), "tools")
                            .WithFiles(GetRunnerDllFiles(c), "tools")
                            .WithDependency("NuGetPowerTools", version: "0.29")
                            .ToFile(GetConsoleRunnerSpecFile(c)));
            }
        }

        private static IEnumerable<IFile> GetLibFiles(TaskContext<BuildRosaliaContext> c)
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

        private static IFile GetConsoleRunnerSpecFile(TaskContext<BuildRosaliaContext> c)
        {
            return c.Data.Src.GetFile("Rosalia.Runner.Console\\Rosalia.Runner.Console.nuspec");
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