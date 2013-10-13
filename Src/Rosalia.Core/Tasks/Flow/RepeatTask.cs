namespace Rosalia.Core.Tasks.Flow
{
    using System;
    using System.Collections.Generic;
    using Rosalia.Core.Context;
    using Rosalia.Core.Fluent;

    public class RepeatTask<TEnumerableItem> : AbstractNodeTask
    {
        private Func<TaskContext, IEnumerable<TEnumerableItem>> _enumerable;
        private Func<TaskContext, TEnumerableItem, ITask> _action;

        public override IEnumerable<ITask> Children
        {
            get { yield break; }
        }

        public RepeatTask<TEnumerableItem> ForEach(Func<TaskContext, IEnumerable<TEnumerableItem>> enumerable)
        {
            _enumerable = enumerable;
            return this;
        }

        public RepeatTask<TEnumerableItem> Do(Func<TaskContext, TEnumerableItem, ITask> action)
        {
            _action = action;
            return this;
        }

        protected override void Execute(ResultBuilder resultBuilder, TaskContext context)
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