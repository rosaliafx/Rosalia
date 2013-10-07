namespace Rosalia.Build.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using Rosalia.Core.Context;
    using Rosalia.Core.FileSystem;

    public static class ContextExtensions
    {
        public static IEnumerable<IDirectory> FindTaskLibDirectories(this BuildRosaliaContext data)
        {
            return
                data.Src.Directories.Where(
                    d =>
                    d.Name.StartsWith("Rosalia.TaskLib") && (!d.Name.EndsWith("Tests")) &&
                    d.Name != "Rosalia.TaskLib.Standard");
        }

        public static IEnumerable<IFile> GetCoreLibFiles(this TaskContext c, BuildRosaliaContext data)
        {
            return c.FileSystem.SearchFilesIn(data.RosaliaRunnerConsoleBin)
                .IncludeByFileName(
                    "Rosalia.Core.dll",
                    "Rosalia.TaskLib.Standard.dll");
        }

        public static IEnumerable<IFile> GetRunnerDllFiles(this TaskContext c, BuildRosaliaContext data)
        {
            return c.FileSystem.SearchFilesIn(data.RosaliaRunnerConsoleBin)
                .IncludeByFileName(
                    "Rosalia.exe",
                    "Rosalia.Core.dll",
                    "Rosalia.Core.Watchers.dll",
                    "Rosalia.Runner.dll",
                    "Rosalia.TaskLib.Standard.dll",
                    "Rosalia.TaskLib.MsBuild.dll");
        }
    }
}