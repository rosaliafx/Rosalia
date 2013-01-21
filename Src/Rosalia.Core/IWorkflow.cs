namespace Rosalia.Core
{
    using Rosalia.Core.FileSystem;
    using Rosalia.Core.Result;

    public interface IWorkflow : IWorkflowEventsAware
    {
        IDirectory WorkDirectory { get; set; }

        int TasksCount { get; }

        ExecutionResult Execute(object inputData);

        void Init(); 
    }
}