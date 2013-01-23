namespace Rosalia.Core.Fluent
{
    using System;
    using Rosalia.Core.Context;

    public class ConditionBuilder<T>
    {
        private readonly ForkTask<T> _fork;
        private readonly Predicate<TaskContext<T>> _condition;

        public ConditionBuilder(ForkTask<T> fork, Predicate<TaskContext<T>> condition)
        {
            _fork = fork;
            _condition = condition;
        }

        public ForkTask<T> Then(ITask<T> child)
        {
            _fork.Add(_condition, child);
            return _fork;
        }

        public ForkTask<T> Then(Action<ResultBuilder, TaskContext<T>> payload)
        {
            _fork.Add(_condition, payload);
            return _fork;
        }
    }
}