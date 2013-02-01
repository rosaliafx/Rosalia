namespace Rosalia.Build.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using Rosalia.Core.Context;
    using Rosalia.Core.FileSystem;

    public static class ContextExtensions
    {
         public static IEnumerable<IDirectory> FindTaskLibDirectories(this TaskContext<BuildRosaliaContext> context)
         {
             return
                 context.Data.Src.Directories.Where(
                     d =>
                     d.Name.StartsWith("Rosalia.TaskLib") && (!d.Name.EndsWith("Tests")) &&
                     d.Name != "Rosalia.TaskLib.Standard");
         }

        public static IEnumerable<IFile> GetCoreLibFiles(this TaskContext<BuildRosaliaContext> c)
        {
            return c.FileSystem.SearchFilesIn(c.Data.RosaliaRunnerConsoleBin)
                .IncludeByFileName(
                    "Rosalia.Core.dll",
                    "Rosalia.TaskLib.Standard.dll");
        }

        public static IEnumerable<IFile> GetRunnerDllFiles(this TaskContext<BuildRosaliaContext> c)
        {
            return c.FileSystem.SearchFilesIn(c.Data.RosaliaRunnerConsoleBin)
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