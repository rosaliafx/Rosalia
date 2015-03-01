using System;

namespace Rosalia.TestingSupport.Executables
{
    using Rosalia.Core.Tasks;
    using Rosalia.Core.Tasks.Results;

    public class FailureTask<T> : AbstractTask<T> where T : class
    {
        private readonly Exception _exception;

        public FailureTask() : this(null)
        {
        }

        public FailureTask(Exception exception)
        {
            _exception = exception;
        }

        protected override ITaskResult<T> SafeExecute(TaskContext context)
        {
            return new FailureResult<T>(_exception);
        }
    }
}