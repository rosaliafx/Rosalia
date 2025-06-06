namespace Rosalia.Build.Helpers
{
    using Rosalia.Core.Tasks;
    using Rosalia.TaskLib.NuGet.Tasks;

    public static class SpecInputExtensions
    {
        public static GenerateNuGetSpecTask FillCommonProperties(this GenerateNuGetSpecTask input, string version)
        {
            return input
                .Version(version)
                .Authors("Eugene Guryanov")
                .ProjectUrl("https://github.com/rosaliafx/Rosalia")
                .Tags("automation", "build", "msbuild", "nant", "psake");
        }

        public static GenerateNuGetSpecTask FillTaskLibProperties(this GenerateNuGetSpecTask input, BuildData data, string version, string taskLib)
        {
            var projectDirectory = data.Src.GetDirectory(string.Format("Rosalia.TaskLib.{0}", taskLib));

            return input
                .Id(string.Format("Rosalia.TaskLib.{0}", taskLib))
                .Description(string.Format("{0} tasks bundle for Rosalia.", taskLib))
                .Tags("rosalia", "tasklib", taskLib.ToLower())
                .WithFile(
                   projectDirectory.GetDirectory("bin").GetDirectory(data.Configuration).GetDirectory(BuildData.LibsTargetFramework).GetFile(string.Format("Rosalia.TaskLib.{0}.dll", taskLib)),
                   "lib")
                .WithDependency("Rosalia.Core", version)
                .WithDependenciesFromPackagesConfig(projectDirectory);
                //.ToFile(data.Artifacts.GetFile(string.Format("Rosalia.TaskLib.{0}.nuspec", taskLib)));
        }
    }
}