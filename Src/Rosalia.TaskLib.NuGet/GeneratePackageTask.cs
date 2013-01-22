namespace Rosalia.TaskLib.NuGet
{
    using System;
    using Rosalia.Core;
    using Rosalia.Core.FileSystem;
    using Rosalia.Core.Fluent;
    using Rosalia.TaskLib.Standard;

    public class GeneratePackageTask<T> : ExternalToolTask<T, object, object>
    {
        private readonly Func<ExecutionContext<T>, IFile> _specFile;

        public GeneratePackageTask(Func<ExecutionContext<T>, IFile> specFile)
        {
            _specFile = specFile;
        }

        protected override object CreateResult(int exitCode, ResultBuilder resultBuilder)
        {
            return null;
        }

        protected override string GetToolPath(object input, ExecutionContext<T> context)
        {
            return "nuget";
        }

        protected override string GetToolArguments(object input, ExecutionContext<T> context)
        {
            return string.Format("pack {0}", _specFile(context).Name);
        }

        protected override IDirectory GetToolWorkDirectory(ExecutionContext<T> context)
        {
            return _specFile(context).Directory;
        }
    }
}