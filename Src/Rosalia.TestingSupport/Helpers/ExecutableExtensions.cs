namespace Rosalia.TestingSupport.Helpers
{
    using Rosalia.Core.Engine.Execution;
    using Rosalia.Core.Tasks;
    using Rosalia.Core.Tasks.Results;

    public static class ExecutableExtensions
    {
        public static ITaskResult<T> Execute<T>(this ITask<T> task) where T : class
        {
            IExecutionStrategy executionStrategy = new ParallelExecutionStrategy();
            TaskContext context = new TaskContext(executionStrategy, new SimpleLogRenderer());

            return task.Execute(context);
        } 
    }
}