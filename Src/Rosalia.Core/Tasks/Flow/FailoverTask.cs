namespace Rosalia.Core.Tasks.Flow
{
    using System.Collections.Generic;
    using Rosalia.Core.Context;
    using Rosalia.Core.Fluent;

    public class FailoverTask<TTarget, TFailover> : AbstractNodeTask
        where TTarget : ITask
        where TFailover : ITask
    {
        private readonly TTarget _targetTask;
        private readonly TFailover _failoverTask;

        public FailoverTask(TTarget targetTask, TFailover failoverTask)
        {
            _targetTask = targetTask;
            _failoverTask = failoverTask;
        }

        public override bool HasChildren
        {
            get { return true; }
        }

        public override IEnumerable<ITask> Children
        {
            get
            {
                yield return Target;
                yield return Failover;
            }
        }

        public TTarget Target
        {
            get { return _targetTask; }
        }

        public TFailover Failover
        {
            get { return _failoverTask; }
        }

        protected override void Execute(ResultBuilder resultBuilder, TaskContext context)
        {
            var targetTaskResult = ExecuteChild(Target, context);
            if (targetTaskResult.ResultType != ResultType.Success)
            {
                var failOverTaskResult = ExecuteChild(Failover, context);
                ApplyChildResult(failOverTaskResult, resultBuilder);
            }
        }
    }
}