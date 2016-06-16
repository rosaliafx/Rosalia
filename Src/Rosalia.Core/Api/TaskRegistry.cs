namespace Rosalia.Core.Api
{
    using Rosalia.Core.Interception;
    using Rosalia.Core.Tasks;
    using Rosalia.Core.Tasks.Futures;
    using Rosalia.Core.Tasks.Results;

    public abstract class TaskRegistry<T>: AbstractTasksRegistry, ITaskRegistry<T>, ITaskRegistryInterceptor<T> where T : class
    {
        protected abstract ITaskFuture<T> RegisterTasks();

        public RegisteredTasks GetRegisteredTasks()
        {
            var resultTaskId = RegisterTasks().Identity;
            return new RegisteredTasks(
                TaskMap.Map,
                resultTaskId,
                GetDefaultTasks() + resultTaskId);
        }

        public virtual ITaskResult<T> AfterExecute(TaskContext context, ITaskResult<T> result)
        {
            return result;
        }
    }

    public abstract class TaskRegistry : AbstractTasksRegistry, ITaskRegistry<Nothing>, ITaskRegistryInterceptor<Nothing>
    {
        public Identity ResultTaskId
        {
            get { return null; }
        }

        protected abstract void RegisterTasks();

        public RegisteredTasks GetRegisteredTasks()
        {
            RegisterTasks();
            return new RegisteredTasks(
                TaskMap.Map,
                null,
                GetDefaultTasks());
        }

        public virtual ITaskResult<Nothing> AfterExecute(TaskContext context, ITaskResult<Nothing> result)
        {
            return result;
        }
    }
}