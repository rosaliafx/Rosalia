namespace Rosalia.Core.Tasks.Futures
{
    /// <summary>
    /// A monad used to configure tasks.
    /// </summary>
    public interface ITaskFuture<out T> where T : class
    {
        Identity Identity { get; }

        T FetchValue(TaskContext context);
    }
}