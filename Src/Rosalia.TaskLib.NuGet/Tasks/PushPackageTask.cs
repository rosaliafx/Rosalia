namespace Rosalia.TaskLib.NuGet.Tasks
{
    using System;
    using System.Linq;
    using Rosalia.Core.Context;
    using Rosalia.Core.Fluent;
    using Rosalia.TaskLib.NuGet.Input;
    using Rosalia.TaskLib.Standard;

    public class PushPackageTask<T> : ExternalToolTask<T, PushInput, object>
    {
        public PushPackageTask()
        {
        }

        public PushPackageTask(PushInput input) : base(input)
        {
        }

        public PushPackageTask(Func<TaskContext<T>, PushInput> contextToInput) : base(contextToInput)
        {
        }

        protected override object CreateResult(int exitCode, ResultBuilder resultBuilder)
        {
            return null;
        }

        protected override string GetToolPath(PushInput input, TaskContext<T> context)
        {
            return "nuget";
        }

        protected override string GetToolArguments(PushInput input, TaskContext<T> context)
        {
            return string.Format(
                "push \"{1}\" {0} {2} -NonInteractive", 
                input.ApiKey,
                input.PackageFile.AbsolutePath, 
                string.Join(" ", input.Options.Select(option => option.CommandLinePart)));
        }
    }
}