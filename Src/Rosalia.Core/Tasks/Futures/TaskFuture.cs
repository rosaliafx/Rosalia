namespace Rosalia.Core.Tasks.Futures
{
    public abstract class TaskFuture<T> : ITaskFuture<T> where T : class
    {
        public abstract Identity Identity { get; }

        public abstract T FetchValue(TaskContext context);
    }
}