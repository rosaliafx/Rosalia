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

    public class BuildRosaliaWorkflow : GenericWorkflow<BuildRosaliaContext>
    {
        public override void RegisterTasks()
        {
            /* ======================================================================================== */
            Register(
                name: "Initialize context",
                task: () =>
                {
                    Data.Configuration = "Debug";
                    Data.ProjectRootDirectory = Context.WorkDirectory.Parent.Parent;
                    Data.Src = Data.ProjectRootDirectory.GetDirectory("Src");
                    Data.SolutionFile = Data.Src.GetFile("Rosalia.sln");
                    Data.Artifacts = Data.ProjectRootDirectory.GetDirectory("Artifacts");
                });

            /* ======================================================================================== */
            Register(
                name: "Get current version from git",
                task: new GetVersionTask(),
                afterExecute: task =>
                {
                    Data.Version = task.Result.Tag.Replace("v", string.Empty) + "." + task.Result.CommitsCount;
                });

            /* ======================================================================================== */
            Register(
                name: "Generate common assembly info file",
                task: new GenerateAssemblyInfo()
                    .WithAttribute(_ => new AssemblyProductAttribute("Rosalia")),
                beforeExecute: task => task
                    .WithAttribute(_ => new AssemblyVersionAttribute(Data.Version))
                    .WithAttribute(_ => new AssemblyFileVersionAttribute(Data.Version))
                    .ToFile(Data.Src.GetFile("CommonAssemblyInfo.cs")));

            /* ======================================================================================== */
            Register(
                name: "Build solution",
                task: new MsBuildTask(),
                beforeExecute: task => task
                    .WithProjectFile(Data.SolutionFile)
                    .WithConfiguration(Data.Configuration)
                    .WithVerbosityMinimal());

            /* ======================================================================================== */
            Register(
                name: "Clear artifacts",
                task: () => Data.Artifacts.Files.IncludeByExtension("nupkg", "nuspec").DeleteAll());

            /* ======================================================================================== */
            Register(
                name: "Generate spec for Core NuGet package",
                task: new GenerateNuGetSpecTask(),
                beforeExecute: task => task
                    .Id("Rosalia.Core")
                    .FillCommonProperties(Context, Data)
                    .Description("Core libs for Rosalia framework.")
                    .WithFiles(Context.GetCoreLibFiles(Data), "lib")
                    .ToFile(Data.NuSpecRosaliaCore));

            /* ======================================================================================== */
            Register(
                name: "Generate specs for NuGet packages",
                task: new GenerateNuGetSpecTask(),
                beforeExecute: task => task
                    .Id("Rosalia")
                    .FillCommonProperties(Context, Data)
                    .Description("Automation tool that allows writing build, install or other workflows in C#. This package automatically integrates Rosalia to your Class Library project.")
                    .WithFile(Data.RosaliaPackageInstallScript, "tools")
                    .WithFile(Data.RosaliaRunnerConsoleExe, "tools")
                    .WithFiles(Context.GetRunnerDllFiles(Data), "tools")
                    .WithFiles(Data.BuildAssets.Files.IncludeByExtension(".pp"), "content")
                    .WithDependency("Rosalia.Core", Data.Version)
                    .WithDependency("NuGetPowerTools", version: "0.29")
                    .ToFile(Data.NuSpecRosalia));

            /* ======================================================================================== */
            Register(
                name: "Generate spec for Task Libs",
                task: ForEach(
                    () => Data.FindTaskLibDirectories(),
                    directory => Register(
                        name: "Generate spec for directory " + directory.Name,
                        task: new GenerateNuGetSpecTask()
                            .FillCommonProperties(Context, Data)
                            .FillTaskLibProperties(Context, Data, directory.Name.Replace("Rosalia.TaskLib.", string.Empty)))));

            /* ======================================================================================== */
            Register(
                name: "Generate NuGet packages",
                task: ForEach(
                    () => Data.Artifacts.Files.IncludeByExtension(".nuspec"),
                    file => Register(
                        name: "Generate nuget package " + file.Name,
                        task: new GeneratePackageTask
                        {
                            SpecFile = file
                        })));

            /* ======================================================================================== */
            Register(
                name: "Genarate docs",
                task: new BuildDocsTask(),
                beforeExecute: task => task.SourceDirectory = Data.Src);

            /* ======================================================================================== */
            Register(
                name: "Push documentation to GhPages",
                task: new PrepareDocsForDeploymentTask(),
                beforeExecute: task =>
                {
                    if (!Context.WorkDirectory.GetFile("private_data.yaml").Exists)
                    {
                        Result.AddWarning("No private data yaml file. Skip docs deployment.");
                        Result.Succeed();
                    }
                });
        }
    }
}