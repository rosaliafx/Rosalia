namespace Rosalia.TaskLib.NuGet.Tasks
{
    using System;
    using Rosalia.Core.Context;
    using Rosalia.Core.FileSystem;
    using Rosalia.Core.Fluent;
    using Rosalia.TaskLib.Standard.Input;
    using Rosalia.TaskLib.Standard.Tasks;

    public class GeneratePackageTask<T> : ExternalToolTask<T, ExternalToolInput, object>
    {
        private readonly Func<TaskContext<T>, IFile> _specFile;

        public GeneratePackageTask(IFile file)
        {
            _specFile = context => file;
        }

        public GeneratePackageTask(Func<TaskContext<T>, IFile> specFile)
        {
            _specFile = specFile;
        }

        protected override string DefaultToolPath
        {
            get { return "nuget"; }
        }

        protected override object CreateResult(int exitCode, ResultBuilder resultBuilder)
        {
            return null;
        }

        protected override string GetToolArguments(ExternalToolInput input, TaskContext<T> context)
        {
            return string.Format("pack {0}", _specFile(context).Name);
        }

        protected override IDirectory GetToolWorkDirectory(ExternalToolInput input, TaskContext<T> context)
        {
            return _specFile(context).Directory;
        }
    }
}