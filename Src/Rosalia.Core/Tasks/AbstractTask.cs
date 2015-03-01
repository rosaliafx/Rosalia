namespace Rosalia.Core.Tasks
{
    using System;
    using Rosalia.Core.Tasks.Results;

    public abstract class AbstractTask<T> : ITask<T> where T : class
    {
        public ITaskResult<T> Execute(TaskContext context)
        {
            try
            {
                return SafeExecute(context);
            }
            catch (Exception ex)
            {
                return new FailureResult<T>(ex);
            }
        }

        protected abstract ITaskResult<T> SafeExecute(TaskContext context);
    }
}