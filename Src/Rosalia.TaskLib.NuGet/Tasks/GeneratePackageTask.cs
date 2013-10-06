namespace Rosalia.TaskLib.NuGet.Tasks
{
    using Rosalia.Core.Context;
    using Rosalia.Core.FileSystem;
    using Rosalia.Core.Fluent;
    using Rosalia.TaskLib.Standard.Tasks;

    public class GeneratePackageTask<T> : NoResultExternalToolTask<T>
    {
        public IFile SpecFile { get; set; }

        protected override string DefaultToolPath
        {
            get { return "nuget"; }
        }

        protected override string GetToolArguments(TaskContext<T> context, ResultBuilder result)
        {
            return string.Format("pack {0}", SpecFile.Name);
        }

        protected override IDirectory GetToolWorkDirectory(TaskContext<T> context)
        {
            return SpecFile.Directory;
        }
    }
}