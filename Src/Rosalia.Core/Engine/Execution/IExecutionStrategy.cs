namespace Rosalia.Core.Engine.Execution
{
    using System;
    using Rosalia.Core.Engine.Composing;
    using Rosalia.Core.Tasks;
    using Rosalia.Core.Tasks.Results;

    public interface IExecutionStrategy
    {
//        ITaskResult<IResultsStorage> Execute(Layer[] layers, TaskContext initialContext);

        ITaskResult<IdentityWithResult[]> Execute(Layer layer, Func<Identity, TaskContext> contextFactory);
    }
}