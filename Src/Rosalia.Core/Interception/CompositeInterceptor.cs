namespace Rosalia.Core.Interception
{
    using System;
    using Rosalia.Core.Tasks;
    using Rosalia.Core.Tasks.Results;

    public class CompositeInterceptor : ITaskInterceptor
    {
        private readonly ITaskInterceptor[] _nestedInterceptors;

        public CompositeInterceptor(params ITaskInterceptor[] nestedInterceptors)
        {
            _nestedInterceptors = nestedInterceptors;
        }

        public void BeforeTaskExecute(Identity id, ITask<object> task, TaskContext context)
        {
            try
            {
                foreach (ITaskInterceptor nestedInterceptor in _nestedInterceptors)
                {
                    nestedInterceptor.BeforeTaskExecute(id, task, context);
                }
            }
            catch (Exception ex)
            {
                context.Log.Error(ex.Message);
            }
        }

        public void AfterTaskExecute(Identity id, ITask<object> task, TaskContext context, ITaskResult<object> result)
        {
            try
            {
                foreach (ITaskInterceptor nestedInterceptor in _nestedInterceptors)
                {
                    nestedInterceptor.AfterTaskExecute(id, task, context, result);
                }
            }
            catch (Exception ex)
            {
                context.Log.Error(ex.Message);
            }
        }
    }
}