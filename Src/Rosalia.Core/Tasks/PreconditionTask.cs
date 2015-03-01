namespace Rosalia.Core.Tasks
{
    using System;
    using Rosalia.Core.Tasks.Results;

    public class PreconditionTask<T>: AbstractTask<T> where T : class
    {
        private readonly ITask<T> _nestedTask;
        private readonly Func<bool> _predicate;
        private readonly Func<T> _defaultValueProvider;

        public PreconditionTask(
            ITask<T> nestedTask, 
            Func<bool> predicate,
            T defaultValue) : this(nestedTask, predicate, () => defaultValue)
        {
        }

        public PreconditionTask(
            ITask<T> nestedTask, 
            Func<bool> predicate) : this(nestedTask, predicate, () => default(T))
        {
        }

        public PreconditionTask(
            ITask<T> nestedTask, 
            Func<bool> predicate, 
            Func<T> defaultValueProvider)
        {
            _nestedTask = nestedTask;
            _predicate = predicate;
            _defaultValueProvider = defaultValueProvider;
        }

        protected override ITaskResult<T> SafeExecute(TaskContext context)
        {
            if (_predicate.Invoke())
            {
                return _nestedTask.Execute(context);
            }

            return new SuccessResult<T>(_defaultValueProvider.Invoke());
        }
    }
}