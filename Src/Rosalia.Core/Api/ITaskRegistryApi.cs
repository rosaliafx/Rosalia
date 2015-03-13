namespace Rosalia.Core.Api
{
    using System;
    using Rosalia.Core.Api.Behaviors;
    using Rosalia.Core.Tasks;
    using Rosalia.Core.Tasks.Futures;
    using Rosalia.Core.Tasks.Results;

    public interface ITaskRegistryApi
    {
        ITaskFuture<T> Task<T>(
            string id,
            ITask<T> task,
            params ITaskBehavior[] behaviors) where T : class;

        ITaskFuture<T> Task<T>(
            string id,
            Func<TaskContext, ITaskResult<T>> task,
            params ITaskBehavior[] behaviors) where T : class;

        ITaskFuture<T> Task<T>(
            string id,
            Func<ITaskResult<T>> task,
            params ITaskBehavior[] behaviors) where T : class;

        ITaskFuture<T> Task<T>(
            string id,
            Func<TaskContext, ITask<T>> task,
            params ITaskBehavior[] behaviors) where T : class;

        ITaskFuture<T> Task<T>(
            string id,
            ITaskFuture<T> task,
            params ITaskBehavior[] behaviors) where T : class;

        ITaskFuture<Nothing> Task(
            string id,
            Action<TaskContext> task,
            params ITaskBehavior[] behaviors);

        ITaskFuture<Nothing> Task(
            string id,
            Action task,
            params ITaskBehavior[] behaviors);

        ITaskFuture<T> Task<T>(
            string id,
            ITaskRegistry<T> task,
            params ITaskBehavior[] behaviors) where T : class;

        ITaskFuture<T> Task<T>(
            string id,
            Func<TaskContext, ITaskRegistry<T>> task,
            params ITaskBehavior[] behaviors) where T : class;
    }
}