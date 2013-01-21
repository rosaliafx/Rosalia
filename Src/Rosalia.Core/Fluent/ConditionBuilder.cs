namespace Rosalia.Core.Fluent
{
    using System;

    public class ConditionBuilder<T>
    {
        private readonly ForkTask<T> _fork;
        private readonly Predicate<ExecutionContext<T>> _condition;

        public ConditionBuilder(ForkTask<T> fork, Predicate<ExecutionContext<T>> condition)
        {
            _fork = fork;
            _condition = condition;
        }

        public ForkTask<T> Then(ITask<T> child)
        {
            _fork.Add(_condition, child);
            return _fork;
        }

        public ForkTask<T> Then(Action<ResultBuilder, ExecutionContext<T>> payload)
        {
            _fork.Add(_condition, payload);
            return _fork;
        }
    }
}