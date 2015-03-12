namespace Rosalia.Build
{
    using System.Reflection;
    using System.Runtime.Remoting.Contexts;
    using Rosalia.Build.Helpers;
    using Rosalia.Core.Api;
    using Rosalia.Core.Tasks.Futures;
    using Rosalia.Core.Tasks.Results;
    using Rosalia.FileSystem;
    using Rosalia.TaskLib.AssemblyInfo;
    using Rosalia.TaskLib.Git.Tasks;
    using Rosalia.TaskLib.MsBuild;
    using Rosalia.TaskLib.NuGet.Tasks;

    public class BuildWorkflow : Workflow
    {
        protected override void RegisterTasks()
        {
            /* ======================================================================================== */
            var solutionTreeTask = Task(
                "Initialize context",
                context =>
                {
                    var projectRoot = context.WorkDirectory.Parent.Parent;
                    var src = projectRoot.GetDirectory("Src");

                    return new BuildData
                    {
                        Configuration = "Debug",
                        ProjectRootDirectory = projectRoot,
                        Src = src,
                        SolutionFile = src.GetFile("Rosalia.sln"),
                        Artifacts = projectRoot.GetDirectory("Artifacts")
                    }.AsTaskResult();
                });

            /* ======================================================================================== */
            
            var gitVersionTask = Task(
                "gitVersion",
                new GetVersionTask());

            /* ======================================================================================== */

            var solutionVersionTask = Task(
                "solutionVersion",
                from value in gitVersionTask
                select (value.Tag.Replace("v", string.Empty) + "." + value.CommitsCount).AsTaskResult());

            /* ======================================================================================== */

            ITaskFuture<Nothing> generateAssemblyInfoTask = Task(
                "Generate common assembly info file",
                
                from version in solutionVersionTask
                from data in solutionTreeTask
                select new GenerateAssemblyInfo(data.Src["CommonAssemblyInfo.cs"])
                {
                    Attributes =
                    {
                        _ => new AssemblyProductAttribute("Rosalia"),
                        _ => new AssemblyVersionAttribute(version),
                        _ => new AssemblyFileVersionAttribute(version)
                    }
                }.AsExecutable());

            /* ======================================================================================== */

            var buildSolution = Task(
                "Build solution",
                from data in solutionTreeTask
                select new MsBuildTask
                {
                    ProjectFile = data.SolutionFile,
                    Switches =
                    {
                        MsBuildSwitch.Configuration(data.Configuration)
                    }
                }.AsExecutable(),
                DependsOn(generateAssemblyInfoTask));

            /* ======================================================================================== */
            
            var clearArtifacts = Task(
                "Clear artifacts",
                from data in solutionTreeTask
                select _ =>
                {
                    data.Artifacts.Files.IncludeByExtension("nupkg", "nuspec").DeleteAll();
                });

            /* ======================================================================================== */
            var nuspecCore = Task(
                "nuspecCore",
                
                from data in solutionTreeTask
                from version in solutionVersionTask
                select new GenerateNuGetSpecTask(data.NuSpecRosaliaCore)
                    .Id("Rosalia.Core")
                    .FillCommonProperties(version)
                    .Description("Core libs for Rosalia framework.")
                    .WithFiles(data.GetCoreLibFiles(), "lib")
                    .AsExecutable(),
                    
                DependsOn(buildSolution),
                DependsOn(clearArtifacts));

            /* ======================================================================================== */
            
            Task(
                "nuspecRosaliaExe",

                from data in solutionTreeTask
                from version in solutionVersionTask
                select new GenerateNuGetSpecTask(data.NuSpecRosalia)
                    .Id("Rosalia")
                    .FillCommonProperties(version)
                    .Description("Automation tool that allows writing build, install or other workflows in C#. This package automatically integrates Rosalia to your Class Library project.")
                    .WithFile(data.RosaliaPackageInstallScript, "tools")
                    .WithFile(data.RosaliaRunnerConsoleExe, "tools")
                    .WithFiles(data.GetRunnerDllFiles(), "tools")
                    .WithFiles(data.BuildAssets.Files.IncludeByExtension(".pp"), "content")
                    .WithDependency("Rosalia.Core", version)
                    .WithDependency("NuGetPowerTools", version: "0.29")
                    .AsExecutable(),
                    
                DependsOn(nuspecCore));

//            /* ======================================================================================== */
//            Register(
//                name: "Generate spec for Task Libs",
//                task: ForEach(
//                    () => Data.FindTaskLibDirectories(),
//                    directory => Register(
//                        name: "Generate spec for directory " + directory.Name,
//                        task: new GenerateNuGetSpecTask()
//                            .FillCommonProperties(Context, Data)
//                            .FillTaskLibProperties(Context, Data, directory.Name.Replace("Rosalia.TaskLib.", string.Empty)))));
//
//            /* ======================================================================================== */
//            Register(
//                name: "Generate NuGet packages",
//                task: ForEach(
//                    () => Data.Artifacts.Files.IncludeByExtension(".nuspec"),
//                    file => Register(
//                        name: "Generate nuget package " + file.Name,
//                        task: new GeneratePackageTask
//                        {
//                            SpecFile = file
//                        })));
//
//            /* ======================================================================================== */
//            Register(
//                name: "Genarate docs",
//                task: new BuildDocsTask(),
//                beforeExecute: task => task.SourceDirectory = Data.Src);
//
//            /* ======================================================================================== */
//            Register(
//                name: "Push documentation to GhPages",
//                task: new PrepareDocsForDeploymentTask(),
//                beforeExecute: task =>
//                {
//                    if (!Context.WorkDirectory.GetFile("private_data.yaml").Exists)
//                    {
//                        Result.AddWarning("No private data yaml file. Skip docs deployment.");
//                        Result.Succeed();
//                    }
//                });
        }
    }
}