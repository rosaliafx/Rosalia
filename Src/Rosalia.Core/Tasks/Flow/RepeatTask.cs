namespace Rosalia.Core.Tasks.Flow
{
    using System;
    using System.Collections.Generic;
    using Rosalia.Core.Context;
    using Rosalia.Core.Fluent;

    public class RepeatTask<T, TTask, TEnumerableItem> : AbstractNodeTask<T> where TTask : ITask<T>
    {
        private Func<TaskContext<T>, IEnumerable<TEnumerableItem>> _enumerable;
        private Func<TaskContext<T>, TEnumerableItem, TTask> _action;

        public override IEnumerable<ITask<T>> Children
        {
            get { yield break; }
        }

        public RepeatTask<T, TTask, TEnumerableItem> ForEach(Func<TaskContext<T>, IEnumerable<TEnumerableItem>> enumerable)
        {
            _enumerable = enumerable;
            return this;
        }

        public RepeatTask<T, TTask, TEnumerableItem> Do(Func<TaskContext<T>, TEnumerableItem, TTask> action)
        {
            _action = action;
            return this;
        }

        protected override void Execute(ResultBuilder resultBuilder, TaskContext<T> context)
        {
            if (_action == null)
            {
                throw new Exception("Action is not set");
            }

            if (_enumerable == null)
            {
                throw new Exception("Enumeration is not set");
            }

            foreach (var item in _enumerable.Invoke(context))
            {
                var task = _action.Invoke(context, item);
                var childResult = ExecuteChild(task, context);
                if (childResult.ResultType != ResultType.Success)
                {
                    resultBuilder.Fail();
                    return;
                }
            }
        }
    }
}