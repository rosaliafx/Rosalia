namespace Rosalia.Core.Engine.Execution
{
    using System;
    using Rosalia.Core.Engine.Composing;
    using Rosalia.Core.Interception;
    using Rosalia.Core.Tasks;
    using Rosalia.Core.Tasks.Results;

    public interface IExecutionStrategy
    {
        ITaskResult<IdentityWithResult[]> Execute(
            Layer layer, 
            Func<Identity, TaskContext> contextFactory,
            ITaskInterceptor interceptor);
    }
}