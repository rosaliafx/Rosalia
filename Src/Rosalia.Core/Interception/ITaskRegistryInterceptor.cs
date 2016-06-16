namespace Rosalia.Core.Interception
{
    using Rosalia.Core.Tasks;
    using Rosalia.Core.Tasks.Results;

    /// <summary>
    /// An interceptor to watch task registries (subflows or workflows).
    /// </summary>
    public interface ITaskRegistryInterceptor<T>
    {
        /// <summary>
        /// Executes then the task registry is done.
        /// </summary>
        /// <param name="context">Task context that holds results of all tasks from the registry</param>
        /// <param name="result">Result of task registry execution</param>
        /// <returns>Intercepted result</returns>
        ITaskResult<T> AfterExecute(TaskContext context, ITaskResult<T> result); 
    }
}