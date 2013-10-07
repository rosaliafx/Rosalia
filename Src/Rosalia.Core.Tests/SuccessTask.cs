namespace Rosalia.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using Rosalia.Core.Context;
    using Rosalia.Core.Events;

    public class SuccessTask : ITask
    {
        public event EventHandler<TaskMessageEventArgs> MessagePosted;

        public bool WasExecuted { get; private set; }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<IIdentifiable> IdentifiableChildren { get; private set; }

        public bool HasChildren { get; set; }

        public IEnumerable<ITask> Children { get; set; }

        public Action<TaskContext> BeforeExecute { get; set; }

        public Action<TaskContext> AfterExecute { get; set; }

        public ExecutionResult Execute(TaskContext context)
        {
            WasExecuted = true;
            return new ExecutionResult(ResultType.Success);
        }
    }
}