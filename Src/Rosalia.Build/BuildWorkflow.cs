namespace Rosalia.Build
{
    using System;
    using System.Reflection;
    using Rosalia.Build.Helpers;
    using Rosalia.Core.Api;
    using Rosalia.Core.Tasks;
    using Rosalia.Core.Tasks.Futures;
    using Rosalia.Core.Tasks.Results;
    using Rosalia.TaskLib.AssemblyInfo;
    using Rosalia.TaskLib.Git.Tasks;
    using Rosalia.TaskLib.MsBuild;
    using Rosalia.TaskLib.NuGet.Tasks;
    using Rosalia.TaskLib.Standard.Tasks;

    public class BuildWorkflow : Workflow
    {
        

        protected override void RegisterTasks()
        {
            /* ======================================================================================== */
            var solutionTreeTask = Task(
                "Initialize context",
                context =>
                {
                    var projectRoot = context.WorkDirectory;
                    while (!(projectRoot["Src"].AsDirectory().Exists && projectRoot["Tools"].AsDirectory().Exists))
                    {
                        projectRoot = projectRoot.Parent;
                    }
                    
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
                select new
                {
                    SolutionVersion = value.Tag.Replace("v", string.Empty) + "." + value.CommitsCount,
                    NuGetVersion = value.Tag.Replace("v", string.Empty) + "." + value.CommitsCount + "-alpha"
                }.AsTaskResult());

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
                        _ => new AssemblyVersionAttribute(version.SolutionVersion),
                        _ => new AssemblyFileVersionAttribute(version.SolutionVersion)
                    }
                }.AsTask());

            /* ======================================================================================== */

            var restorePackages = Task(
                "restorePackages",
                from data in solutionTreeTask
                select new ExecTask
                {
                    ToolPath = Type.GetType("Mono.Runtime") != null ? "mono": data.Src[".nuget"]["NuGet.exe"].AsFile().AbsolutePath,
                    Arguments = (Type.GetType("Mono.Runtime") != null ? data.Src[".nuget"]["NuGet.exe"].AsFile().AbsolutePath + " ": "") + "restore " + data.SolutionFile.AbsolutePath
                }.AsTask());

            /* ======================================================================================== */

            var buildSolution = Task(
                "buildSolution",
                from data in solutionTreeTask
                select new MsBuildTask
                {
                    ProjectFile = data.SolutionFile,
                    Switches =
                    {
                        MsBuildSwitch.Configuration(data.Configuration)
                    }
                }.AsTask(),

                DependsOn(generateAssemblyInfoTask),
                DependsOn(restorePackages));

            /* ======================================================================================== */
            
            var clearArtifacts = Task(
                "Clear artifacts",
                from data in solutionTreeTask
                select _ =>
                {
                    data.Artifacts.EnsureExists();
                    data.Artifacts.Files.IncludeByExtension("nupkg", "nuspec").DeleteAll();
                });

            /* ======================================================================================== */
            var nuspecCore = Task(
                "nuspecCore",
                
                from data in solutionTreeTask
                from version in solutionVersionTask
                select new GenerateNuGetSpecTask(data.NuSpecRosaliaCore)
                    .Id("Rosalia.Core")
                    .FillCommonProperties(version.NuGetVersion)
                    .Description("Core libs for Rosalia framework.")
                    .WithFiles(data.GetCoreLibFiles(), "lib")
                    .AsTask(),
                    
                DependsOn(buildSolution),
                DependsOn(clearArtifacts));

            /* ======================================================================================== */
            
            var nuspecRosaliaExe = Task(
                "nuspecRosaliaExe",

                from data in solutionTreeTask
                from version in solutionVersionTask
                select new GenerateNuGetSpecTask(data.NuSpecRosalia)
                    .Id("Rosalia")
                    .FillCommonProperties(version.NuGetVersion)
                    .Description("Automation tool that allows writing build, install or other workflows in C#. This package automatically integrates Rosalia to your Class Library project.")
                    .WithFile(data.RosaliaPackageInstallScript, "tools")
                    .WithFile(data.RosaliaRunnerConsoleExe, "tools")
                    .WithFiles(data.GetRunnerDllFiles(), "tools")
                    .WithFiles(data.BuildAssets.Files.IncludeByExtension(".pp"), "content")
                    .WithDependency("Rosalia.Core", version.NuGetVersion)
                    .WithDependency("NuGetPowerTools", version: "0.29")
                    .AsTask(),
                    
                DependsOn(nuspecCore));

            /* ======================================================================================== */
            ITaskFuture<Nothing> nuspecTaskLibs = Task(
                "nuspecTaskLibs",
                from data in solutionTreeTask
                from version in solutionVersionTask
                select ForEach(data.FindTaskLibDirectories()).Do(
                    tasklibDir =>
                    {
                        var libCode = tasklibDir.Name.Replace("Rosalia.TaskLib.", string.Empty);
                        var taskLibNuspec = string.Format("Rosalia.TaskLib.{0}.nuspec", libCode);

                        return new GenerateNuGetSpecTask(data.Artifacts[taskLibNuspec])
                            .FillCommonProperties(version.NuGetVersion)
                            .FillTaskLibProperties(data, version.NuGetVersion, libCode);
                    },
                    tasklibDir => "nuspec" + tasklibDir.Name.Replace("Rosalia.TaskLib.", string.Empty)),
                
                DependsOn(nuspecRosaliaExe));

            /* ======================================================================================== */

            Task(
                "Generate NuGet packages",
                from data in solutionTreeTask
                select ForEach(data.Artifacts.Files.IncludeByExtension(".nuspec")).Do(
                    file => new GeneratePackageTask(file)
                    {
                        ToolPath = data.Src[".nuget"]["NuGet.exe"].AsFile().AbsolutePath
                    },
                    file => "pack " + file.Name),
                
                Default(),

                DependsOn(nuspecCore),
                DependsOn(nuspecRosaliaExe),
                DependsOn(nuspecTaskLibs));
        }
    }
}