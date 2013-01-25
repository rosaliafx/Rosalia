namespace Rosalia.Core.Fluent
{
    using System;
    using System.Collections.Generic;
    using Rosalia.Core.Context;
    using Rosalia.Core.Tasks.Flow;

    public class RepeaterBuilder<T, TEnumerableItem>
    {
        private readonly Func<TaskContext<T>, IEnumerable<TEnumerableItem>> _enumerableProvider;

        public RepeaterBuilder(Func<TaskContext<T>, IEnumerable<TEnumerableItem>> enumerableProvider)
        {
            _enumerableProvider = enumerableProvider;
        }

        public ITask<T> Do<TTask>(Func<TaskContext<T>, TEnumerableItem, TTask> taskProvider) where TTask : ITask<T>
        {
            return new RepeatTask<T, TTask, TEnumerableItem>()
                .ForEach(_enumerableProvider)
                .Do(taskProvider);
        }
    }
}