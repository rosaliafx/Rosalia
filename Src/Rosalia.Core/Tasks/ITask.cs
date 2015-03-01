namespace Rosalia.Core.Tasks
{
    using Rosalia.Core.Tasks.Results;

    public interface ITask<out T> where T : class
    {
        ITaskResult<T> Execute(TaskContext context);
    }
}