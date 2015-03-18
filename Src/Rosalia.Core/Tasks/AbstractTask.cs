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
                var result = SafeExecute(context);

//                LogResult(context, result);
                return result;
            }
            catch (Exception ex)
            {
                if (UnswallowedExceptionTypes != null)
                {
                    if (UnswallowedExceptionTypes.Any(type => type.IsInstanceOfType(ex)))
                    {
                        throw ex;
                    }
                }

                var result = new FailureResult<T>(ex);

//                LogResult(context, result);

                return result;
            }
        }

//        protected virtual void LogResult(TaskContext context, ITaskResult<T> result)
//        {
//            if (result.IsSuccess)
//            {
//                context.Log.Info("Completed. Result is {0}", result.Data);
//            }
//            else
//            {
//                context.Log.Error(result.Error == null ? "Unknown error occured" : result.Error.Message);
//            }
//        }

        protected abstract ITaskResult<T> SafeExecute(TaskContext context);
    }
}