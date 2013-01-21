namespace Rosalia.Core
{
    using System;
    using Rosalia.Core.Fluent;

    /// <summary>
    /// Simple task with lambda payload for inline instantiation.
    /// </summary>
    public class SimpleTask<T> : AbstractLeafTask<T>
    {
        private readonly Action<ResultBuilder, ExecutionContext<T>> _payload;

        public SimpleTask(Action<ResultBuilder, ExecutionContext<T>> payload)
        {
            _payload = payload;
        }

        protected override void Execute(ResultBuilder resultBuilder, ExecutionContext<T> context)
        {
            _payload(resultBuilder, context);
        }
    }
}