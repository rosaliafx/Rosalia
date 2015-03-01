namespace Rosalia.Core.Tasks
{
    using System;
    using Rosalia.Core.Tasks.Results;

    public class RecoverTask<T> : AbstractTask<T> where T : class
    {
        private readonly ITask<T> _nestedTask;
        private readonly Func<T> _recoverValueProvider;

        public RecoverTask(ITask<T> nestedTask) : this(nestedTask, default(T))
        {
        }

        public RecoverTask(ITask<T> nestedTask, T value) : this(nestedTask, () => value)
        {
        }

        public RecoverTask(ITask<T> nestedTask, Func<T> recoverValueProvider)
        {
            _nestedTask = nestedTask;
            _recoverValueProvider = recoverValueProvider;
        }

        protected override ITaskResult<T> SafeExecute(TaskContext context)
        {
            var nestedExecutableResult = _nestedTask.Execute(context);
            if (nestedExecutableResult.IsSuccess)
            {
                return nestedExecutableResult;
            }

            return new SuccessResult<T>(_recoverValueProvider.Invoke());
        }
    }
}