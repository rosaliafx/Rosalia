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
                task: new GetVersionTask<BuildRosaliaContext>(),
                afterExecute: (context, task) =>
                {
                    context.Data.Version = task.Result.Tag.Replace("v", string.Empty) + "." + task.Result.CommitsCount;
                });

            /* ======================================================================================== */
            Register(
                name: "Generate common assembly info file",
                task: new GenerateAssemblyInfo<BuildRosaliaContext>()
                    .WithAttribute(_ => new AssemblyProductAttribute("Rosalia")),
                beforeExecute: (context, task) => task
                    .WithAttribute(_ => new AssemblyVersionAttribute(context.Data.Version))
                    .WithAttribute(_ => new AssemblyFileVersionAttribute(context.Data.Version))
                    .ToFile(context.Data.Src.GetFile("CommonAssemblyInfo.cs")));

            /* ======================================================================================== */
            Register(
                name: "Build solution",
                task: new MsBuildTask<BuildRosaliaContext>(),
                beforeExecute: (context, task) => task
                    .WithProjectFile(context.Data.SolutionFile)
                    .WithConfiguration(context.Data.Configuration)
                    .WithVerbosityMinimal());

            /* ======================================================================================== */
            Register(
                name: "Clear artifacts",
                task: (builder, context) => context.Data.Artifacts.Files.IncludeByExtension("nupkg", "nuspec").DeleteAll());

            /* ======================================================================================== */
            Register(
                name: "Generate spec for Core NuGet package",
                task: new GenerateNuGetSpecTask<BuildRosaliaContext>(),
                beforeExecute: (context, task) => task
                    .Id("Rosalia.Core")
                    .FillCommonProperties(context)
                    .Description("Core libs for Rosalia framework.")
                    .WithFiles(context.GetCoreLibFiles(), "lib")
                    .ToFile(context.Data.NuSpecRosaliaCore));

            /* ======================================================================================== */
            Register(
                name: "Generate specs for NuGet packages",
                task: new GenerateNuGetSpecTask<BuildRosaliaContext>(),
                beforeExecute: (context, task) => task
                    .Id("Rosalia")
                    .FillCommonProperties(context)
                    .Description("Automation tool that allows writing build, install or other workflows in C#. This package automatically integrates Rosalia to your Class Library project.")
                    .WithFile(context.Data.RosaliaPackageInstallScript, "tools")
                    .WithFile(context.Data.RosaliaRunnerConsoleExe, "tools")
                    .WithFiles(context.GetRunnerDllFiles(), "tools")
                    .WithFiles(context.Data.BuildAssets.Files.IncludeByExtension(".pp"), "content")
                    .WithDependency("Rosalia.Core", context.Data.Version)
                    .WithDependency("NuGetPowerTools", version: "0.29")
                    .ToFile(context.Data.NuSpecRosalia));

            /* ======================================================================================== */
            Register(
                name: "Generate spec for Task Libs",
                task: ForEach(c => c.FindTaskLibDirectories())
                        .Do((context, directory) =>
                            new GenerateNuGetSpecTask<BuildRosaliaContext>()
                                .FillCommonProperties(context)
                                .FillTaskLibProperties(context, directory.Name.Replace("Rosalia.TaskLib.", string.Empty))));

            /* ======================================================================================== */
            Register(
                name: "Generate NuGet packages",
                task: ForEach(c => c.Data.Artifacts.Files.IncludeByExtension(".nuspec"))
                        .Do((context, file) => new GeneratePackageTask<BuildRosaliaContext>
                        {
                            SpecFile = file
                        }));

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