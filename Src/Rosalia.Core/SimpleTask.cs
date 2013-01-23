namespace Rosalia.Core
{
    using System;
    using Rosalia.Core.Context;
    using Rosalia.Core.Fluent;

    /// <summary>
    /// Simple task with lambda payload for inline instantiation.
    /// </summary>
    public class SimpleTask<T> : AbstractLeafTask<T>
    {
        private readonly Action<ResultBuilder, TaskContext<T>> _payload;

        public SimpleTask(Action<ResultBuilder, TaskContext<T>> payload)
        {
            _payload = payload;
        }

        protected override void Execute(ResultBuilder resultBuilder, TaskContext<T> context)
        {
            _payload(resultBuilder, context);
        }
    }
}