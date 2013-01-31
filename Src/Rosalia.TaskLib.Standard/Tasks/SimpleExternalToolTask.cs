namespace Rosalia.TaskLib.Standard.Tasks
{
    using System;
    using Rosalia.Core.Context;
    using Rosalia.Core.Fluent;
    using Rosalia.TaskLib.Standard.Input;

    public class SimpleExternalToolTask<TContext> : ExternalToolTask<TContext, ExternalToolInput, object>
    {
        public SimpleExternalToolTask(Func<TaskContext<TContext>, ExternalToolInput> inputProvider) : base(inputProvider)
        {
        }

        protected override object CreateResult(int exitCode, ResultBuilder resultBuilder)
        {
            return null;
        }
    }
}