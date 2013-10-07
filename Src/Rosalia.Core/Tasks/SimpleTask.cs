namespace Rosalia.Core.Tasks
{
    using System;
    using Rosalia.Core.Context;
    using Rosalia.Core.Fluent;

    /// <summary>
    /// Simple task with lambda payload for inline instantiation.
    /// </summary>
    public class SimpleTask : AbstractLeafTask
    {
        private readonly Action<ResultBuilder, TaskContext> _payload;

        public SimpleTask(Action<ResultBuilder, TaskContext> payload)
        {
            _payload = payload;
        }

        protected override void Execute(ResultBuilder resultBuilder, TaskContext context)
        {
            _payload(resultBuilder, context);
        }
    }
}