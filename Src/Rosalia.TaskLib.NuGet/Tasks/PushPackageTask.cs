namespace Rosalia.TaskLib.NuGet.Tasks
{
    using System;
    using System.Linq;
    using Rosalia.Core.Context;
    using Rosalia.Core.Fluent;
    using Rosalia.TaskLib.NuGet.Input;
    using Rosalia.TaskLib.Standard.Tasks;

    public class PushPackageTask<T> : ExternalToolTask<T, PushInput, object>
    {
        public PushPackageTask()
        {
        }

        public PushPackageTask(PushInput input) : base(input)
        {
        }

        public PushPackageTask(Func<TaskContext<T>, PushInput> inputProvider) : base(inputProvider)
        {
        }

        protected override object CreateResult(int exitCode, ResultBuilder resultBuilder)
        {
            return null;
        }

        protected override string GetToolPath(PushInput input, TaskContext<T> context, ResultBuilder result)
        {
            return "nuget";
        }

        protected override string GetToolArguments(PushInput input, TaskContext<T> context, ResultBuilder result)
        {
            return string.Format(
                "push \"{1}\" {0} {2} -NonInteractive", 
                input.ApiKey,
                input.PackageFile.AbsolutePath, 
                string.Join(" ", input.Options.Select(option => option.CommandLinePart)));
        }
    }
}