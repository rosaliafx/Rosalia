namespace Rosalia.TaskLib.NuGet
{
    using System;
    using Rosalia.Core;
    using Rosalia.Core.Context;
    using Rosalia.Core.FileSystem;
    using Rosalia.Core.Fluent;
    using Rosalia.TaskLib.Standard;

    public class GeneratePackageTask<T> : ExternalToolTask<T, object, object>
    {
        private readonly Func<TaskContext<T>, IFile> _specFile;

        public GeneratePackageTask(Func<TaskContext<T>, IFile> specFile)
        {
            _specFile = specFile;
        }

        protected override object CreateResult(int exitCode, ResultBuilder resultBuilder)
        {
            return null;
        }

        protected override string GetToolPath(object input, TaskContext<T> context)
        {
            return "nuget";
        }

        protected override string GetToolArguments(object input, TaskContext<T> context)
        {
            return string.Format("pack {0}", _specFile(context).Name);
        }

        protected override IDirectory GetToolWorkDirectory(TaskContext<T> context)
        {
            return _specFile(context).Directory;
        }
    }
}