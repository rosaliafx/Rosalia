namespace Rosalia.Build.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using Rosalia.FileSystem;

    public static class ContextExtensions
    {
        public static IEnumerable<IDirectory> FindTaskLibDirectories(this BuildData data)
        {
            return
                data.Src.Directories.Where(
                    d =>
                    d.Name.StartsWith("Rosalia.TaskLib") && (!d.Name.EndsWith("Tests")) &&
                    d.Name != "Rosalia.TaskLib.Standard");
        }

        public static IEnumerable<IFile> GetCoreLibFiles(this BuildData data)
        {
            return data.RosaliaRunnerConsoleBin.SearchFilesIn()
                .IncludeByFileName(
                    "Rosalia.Core.dll",
                    "Rosalia.FileSystem.dll",
                    "Rosalia.TaskLib.Standard.dll");
        }

        public static IEnumerable<IFile> GetRunnerDllFiles(this BuildData data)
        {
            return data.RosaliaRunnerConsoleBin.SearchFilesIn()
                .IncludeByFileName(
                    "Rosalia.exe",
                    "Rosalia.Core.dll",
                    "Rosalia.FileSystem.dll",
                    "Rosalia.TaskLib.Standard.dll",
                    "Rosalia.TaskLib.MsBuild.dll");
        }
    }
}