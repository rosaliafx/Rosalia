namespace Rosalia.Core.Tasks
{
    using System;
    using Rosalia.Core.Tasks.Results;

    public class TransformTask<TInput, TOutput> : AbstractTask<TOutput> 
        where TOutput : class 
        where TInput : class
    {
        private readonly ITask<TInput> _nestedTask;
        private readonly Func<TInput, TOutput> _transformer;

        public TransformTask(ITask<TInput> nestedTask, Func<TInput, TOutput> transformer)
        {
            _nestedTask = nestedTask;
            _transformer = transformer;
        }

        protected override ITaskResult<TOutput> SafeExecute(TaskContext context)
        {
            ITaskResult<TInput> nestedResult = _nestedTask.Execute(context);
            if (!nestedResult.IsSuccess)
            {
                return new FailureResult<TOutput>(nestedResult.Error);
            }

            TOutput transformed = _transformer.Invoke(nestedResult.Data);
            return new SuccessResult<TOutput>(transformed);
        }
    }
}