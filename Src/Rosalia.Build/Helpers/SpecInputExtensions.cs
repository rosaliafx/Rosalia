namespace Rosalia.Build.Helpers
{
    using Rosalia.Core.Context;
    using Rosalia.TaskLib.NuGet.Input;

    public static class SpecInputExtensions
    {
        public static SpecInput FillCommonProperties(this SpecInput input, TaskContext<BuildRosaliaContext> context)
        {
            return input
                .Version(context.Data.Version)
                .Authors("Eugene Guryanov")
                .ProjectUrl("http://rosalia-tool.net")
                .Tags("automation", "build", "msbuild", "nant", "psake");
        }

        public static SpecInput FillTaskLibProperties(this SpecInput input, TaskContext<BuildRosaliaContext> context, string taskLib)
        {
            var projectDirectory = context.Data.Src.GetDirectory(string.Format("Rosalia.TaskLib.{0}", taskLib));

            return input
                .Id(string.Format("Rosalia.TaskLib.{0}", taskLib))
                .Description(string.Format("{0} tasks bundle for Rosalia.", taskLib))
                .Tags("rosalia", "tasklib", taskLib.ToLower())
                .WithFile(
                   projectDirectory.GetDirectory("bin").GetDirectory(context.Data.Configuration).GetFile(string.Format("Rosalia.TaskLib.{0}.dll", taskLib)),
                   "lib")
                .WithDependency("Rosalia.Core", context.Data.Version)
                .WithDependenciesFromPackagesConfig(projectDirectory)
                .ToFile(context.Data.Artifacts.GetFile(string.Format("Rosalia.TaskLib.{0}.nuspec", taskLib)));
        }
    }
}