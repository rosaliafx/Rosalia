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
    }
}