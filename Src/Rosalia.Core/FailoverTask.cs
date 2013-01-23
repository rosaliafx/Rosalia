namespace Rosalia.Core
{
    using System.Collections.Generic;
    using Rosalia.Core.Context;
    using Rosalia.Core.Fluent;

    public class FailoverTask<T> : AbstractNodeTask<T>
    {
        private readonly ITask<T> _targetTask;
        private readonly ITask<T> _failoverTask;

        public FailoverTask(ITask<T> targetTask, ITask<T> failoverTask)
        {
            _targetTask = targetTask;
            _failoverTask = failoverTask;
        }

        public override bool HasChildren
        {
            get { return true; }
        }

        public override IEnumerable<ITask<T>> Children
        {
            get
            {
                yield return _targetTask;
                yield return _failoverTask;
            }
        }

        protected override void Execute(ResultBuilder resultBuilder, TaskContext<T> context)
        {
            var targetTaskResult = ExecuteChild(_targetTask, context);
            if (targetTaskResult.ResultType != ResultType.Success)
            {
                var failOverTaskResult = ExecuteChild(_failoverTask, context);
                ApplyChildResult(failOverTaskResult, resultBuilder);
            }
        }
    }
}