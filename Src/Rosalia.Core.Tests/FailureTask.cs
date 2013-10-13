namespace Rosalia.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using Rosalia.Core.Context;
    using Rosalia.Core.Events;
    using Rosalia.Core.Fluent;

    public class FailureTask : ITask
    {
        public event EventHandler<TaskMessageEventArgs> MessagePosted;

        public Guid Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<IIdentifiable> IdentifiableChildren { get; private set; }

        public bool HasChildren { get; set; }

        public IEnumerable<ITask> Children { get; set; }

        public Action<TaskContext, ResultBuilder> BeforeExecute { get; set; }

        public Action<TaskContext> AfterExecute { get; set; }

        public ExecutionResult Execute(TaskContext context)
        {
            return new ExecutionResult(ResultType.Failure);
        }
    }
}