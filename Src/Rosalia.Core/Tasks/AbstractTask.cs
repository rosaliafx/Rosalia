namespace Rosalia.Core.Tasks
{
    using System;
    using System.Linq;
    using Rosalia.Core.Tasks.Results;

    public abstract class AbstractTask : AbstractTask<Nothing>
    {
        protected ITaskResult<Nothing> Success
        {
            get
            {
                return new SuccessResult<Nothing>(Nothing.Value);
            }
        }
    }

    public abstract class AbstractTask<T> : ITask<T> where T : class
    {
        public Type[] UnswallowedExceptionTypes { get; set; }

        public ITaskResult<T> Execute(TaskContext context)
        {
            try
            {
                return SafeExecute(context);
            }
            catch (Exception ex)
            {
                if (UnswallowedExceptionTypes != null)
                {
                    if (UnswallowedExceptionTypes.Any(type => type.IsInstanceOfType(ex)))
                    {
                        throw;
                    }
                }

                return new FailureResult<T>(ex);
            }
        }

        protected abstract ITaskResult<T> SafeExecute(TaskContext context);
    }
}