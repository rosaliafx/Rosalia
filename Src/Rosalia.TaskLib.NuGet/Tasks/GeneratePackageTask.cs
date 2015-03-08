namespace Rosalia.TaskLib.NuGet.Tasks
{
    using Rosalia.Core.Tasks;
    using Rosalia.FileSystem;
    using Rosalia.TaskLib.Standard.Tasks;

    public class GeneratePackageTask : ExternalToolTask
    {
        private readonly IFile _specFile;

        public GeneratePackageTask(IFile specFile)
        {
            _specFile = specFile;
        }

        public IFile SpecFile
        {
            get { return _specFile; }
        }

        protected override string DefaultToolPath
        {
            get { return "nuget"; }
        }

        protected override string GetToolArguments(TaskContext context)
        {
            return string.Format("pack {0}", SpecFile.Name);
        }

        protected override IDirectory GetToolWorkDirectory(TaskContext context)
        {
            return SpecFile.Directory;
        }
    }
}