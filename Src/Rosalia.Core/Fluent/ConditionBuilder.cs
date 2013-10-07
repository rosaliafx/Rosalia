namespace Rosalia.Core.Fluent
{
    using System;
    using Rosalia.Core.Context;
    using Rosalia.Core.Tasks.Flow;

    public class ConditionBuilder
    {
        private readonly ForkTask _fork;
        private readonly Predicate<TaskContext> _condition;

        public ConditionBuilder(ForkTask fork, Predicate<TaskContext> condition)
        {
            _fork = fork;
            _condition = condition;
        }

        public ForkTask Then(ITask child)
        {
            _fork.Add(_condition, child);
            return _fork;
        }

        public ForkTask Then(Action<ResultBuilder, TaskContext> payload)
        {
            _fork.Add(_condition, payload);
            return _fork;
        }
    }
}