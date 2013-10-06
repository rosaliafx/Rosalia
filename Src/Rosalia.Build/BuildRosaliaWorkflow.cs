namespace Rosalia.Build
{
    using System.Reflection;
    using Rosalia.Build.Helpers;
    using Rosalia.Build.Tasks;
    using Rosalia.Core;
    using Rosalia.TaskLib.AssemblyInfo;
    using Rosalia.TaskLib.Git.Tasks;
    using Rosalia.TaskLib.MsBuild;
    using Rosalia.TaskLib.NuGet.Tasks;

    public class BuildRosaliaWorkflow : Workflow<BuildRosaliaContext>
    {
        public override void RegisterTasks()
        {
            /* ======================================================================================== */
            Register(
                name: "Initialize context",
                task: (builder, context) =>
                {
                    var data = context.Data;

                    data.Configuration = "Debug";
                    data.ProjectRootDirectory = context.WorkDirectory.Parent.Parent;
                    data.Src = data.ProjectRootDirectory.GetDirectory("Src");
                    data.SolutionFile = data.Src.GetFile("Rosalia.sln");
                    data.Artifacts = data.ProjectRootDirectory.GetDirectory("Artifacts");
                });

            /* ======================================================================================== */
            Register(
                name: "Get current version from git",
                task: new GetVersionTask<BuildRosaliaContext>(
                (output, c) =>
                {
                    c.Version = output.Tag.Replace("v", string.Empty) + "." + output.CommitsCount;
                }),
                afterExecute: (context, task) =>
                {
                    //context.Data.Version = task.Re                      
                });

            /* ======================================================================================== */
            Register(
                name: "Generate common assembly info file",
                task: new GenerateAssemblyInfo<BuildRosaliaContext>()
                    .WithAttribute(c => new AssemblyProductAttribute("Rosalia")),
                beforeExecute: (context, task) => task
                    .WithAttribute(_ => new AssemblyVersionAttribute(context.Data.Version))
                    .WithAttribute(_ => new AssemblyFileVersionAttribute(context.Data.Version))
                    .ToFile(context.Data.Src.GetFile("CommonAssemblyInfo.cs")));

            /* ======================================================================================== */
            Register(
                name: "Build solution",
                task: new MsBuildTask<BuildRosaliaContext>()
                        .FillInput(c => new MsBuildInput()
                            .WithProjectFile(c.Data.SolutionFile)
                            .WithConfiguration(c.Data.Configuration)
                            .WithVerbosityMinimal()));

            /* ======================================================================================== */
            Register(
                name: "Clear artifacts",
                task: (builder, context) => context.Data.Artifacts.Files.IncludeByExtension("nupkg", "nuspec").DeleteAll());

            /* ======================================================================================== */
            Register(
                name: "Generate spec for Core NuGet package",
                task: new GenerateNuGetSpecTask<BuildRosaliaContext>((c, input) =>
                        input
                            .Id("Rosalia.Core")
                            .FillCommonProperties(c)
                            .Description("Core libs for Rosalia framework.")
                            .WithFiles(c.GetCoreLibFiles(), "lib")
                            .ToFile(c.Data.NuSpecRosaliaCore)));

            /* ======================================================================================== */
            Register(
                name: "Generate specs for NuGet packages",
                task: new GenerateNuGetSpecTask<BuildRosaliaContext>((c, input) =>
                        input
                            .Id("Rosalia")
                            .FillCommonProperties(c)
                            .Description("Automation tool that allows writing build, install or other workflows in C#. This package automatically integrates Rosalia to your Class Library project.")
                            .WithFile(c.Data.RosaliaPackageInstallScript, "tools")
                            .WithFile(c.Data.RosaliaRunnerConsoleExe, "tools")
                            .WithFiles(c.GetRunnerDllFiles(), "tools")
                            .WithFiles(c.Data.BuildAssets.Files.IncludeByExtension(".pp"), "content")
                            .WithDependency("Rosalia.Core", c.Data.Version)
                            .WithDependency("NuGetPowerTools", version: "0.29")
                            .ToFile(c.Data.NuSpecRosalia)));

            /* ======================================================================================== */
            Register(
                name: "Generate spec for Task Libs",
                task: ForEach(c => c.FindTaskLibDirectories())
                        .Do((context, directory) =>
                            new GenerateNuGetSpecTask<BuildRosaliaContext>((taskContext, input) => input
                                .FillCommonProperties(taskContext)
                                .FillTaskLibProperties(taskContext, directory.Name.Replace("Rosalia.TaskLib.", string.Empty)))));

            /* ======================================================================================== */
            Register(
                name: "Generate NuGet packages",
                task: ForEach(c => c.Data.Artifacts.Files.IncludeByExtension(".nuspec"))
                        .Do((context, file) => new GeneratePackageTask<BuildRosaliaContext>(file)));

            /* ======================================================================================== */
            Register(
                name: "Genarate docs",
                task: new BuildDocsTask());

            /* ======================================================================================== */
            Register(
                name: "Push documentation to GhPages",
                task: If(c => c.WorkDirectory.GetFile("private_data.yaml").Exists).Then(new PrepareDocsForDeploymentTask()));
        }
    }
}