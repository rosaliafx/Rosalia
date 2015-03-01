namespace Rosalia.Core.Engine.Execution
{
    using Rosalia.Core.Engine.Composing;
    using Rosalia.Core.Logging;
    using Rosalia.Core.Tasks;
    using Rosalia.Core.Tasks.Results;

    public interface IExecutionStrategy
    {
        ITaskResult<IResultsStorage> Execute(Layer[] layers, ILogRenderer logRenderer);
    }
}