namespace Rosalia.Build.Helpers
{
    using Rosalia.Core.Context;
    using Rosalia.TaskLib.NuGet.Tasks;

    public static class SpecInputExtensions
    {
        public static GenerateNuGetSpecTask FillCommonProperties(this GenerateNuGetSpecTask input, TaskContext context, BuildRosaliaContext data)
        {
            return input
                .Version(data.Version)
                .Authors("Eugene Guryanov")
                .ProjectUrl("http://rosalia-tool.net")
                .Tags("automation", "build", "msbuild", "nant", "psake");
        }

        public static GenerateNuGetSpecTask FillTaskLibProperties(this GenerateNuGetSpecTask input, TaskContext context, BuildRosaliaContext data, string taskLib)
        {
            var projectDirectory = data.Src.GetDirectory(string.Format("Rosalia.TaskLib.{0}", taskLib));

            return input
                .Id(string.Format("Rosalia.TaskLib.{0}", taskLib))
                .Description(string.Format("{0} tasks bundle for Rosalia.", taskLib))
                .Tags("rosalia", "tasklib", taskLib.ToLower())
                .WithFile(
                   projectDirectory.GetDirectory("bin").GetDirectory(data.Configuration).GetFile(string.Format("Rosalia.TaskLib.{0}.dll", taskLib)),
                   "lib")
                .WithDependency("Rosalia.Core", data.Version)
                .WithDependenciesFromPackagesConfig(projectDirectory)
                .ToFile(data.Artifacts.GetFile(string.Format("Rosalia.TaskLib.{0}.nuspec", taskLib)));
        }
    }
}