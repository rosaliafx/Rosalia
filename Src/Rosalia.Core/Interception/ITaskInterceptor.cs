namespace Rosalia.Core.Interception
{
    using Rosalia.Core.Tasks;
    using Rosalia.Core.Tasks.Results;

    public interface ITaskInterceptor
    {
        void BeforeTaskExecute(Identity id, ITask<object> task, TaskContext context);

        void AfterTaskExecute(Identity id, ITask<object> task, TaskContext context, ITaskResult<object> result);
    }
}