namespace Rosalia.Core.Tasks
{
    using System;
    using Rosalia.Core.Tasks.Results;

    public class FuncTask<T> : AbstractTask<T> where T : class
    {
        private readonly Func<TaskContext, ITaskResult<T>> _payload;

        public FuncTask(Func<TaskContext, ITaskResult<T>> payload)
        {
            _payload = payload;
        }

        protected override ITaskResult<T> SafeExecute(TaskContext context)
        {
            return _payload.Invoke(context);
        }
    }
}