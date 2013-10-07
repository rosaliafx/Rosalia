namespace Rosalia.Core.Fluent
{
    using System;
    using System.Collections.Generic;
    using Rosalia.Core.Context;
    using Rosalia.Core.Tasks.Flow;

    public class RepeaterBuilder<TEnumerableItem>
    {
        private readonly Func<TaskContext, IEnumerable<TEnumerableItem>> _enumerableProvider;

        public RepeaterBuilder(Func<TaskContext, IEnumerable<TEnumerableItem>> enumerableProvider)
        {
            _enumerableProvider = enumerableProvider;
        }

        public ITask Do<TTask>(Func<TaskContext, TEnumerableItem, TTask> taskProvider) where TTask : ITask
        {
            return new RepeatTask<TTask, TEnumerableItem>()
                .ForEach(_enumerableProvider)
                .Do(taskProvider);
        }
    }
}