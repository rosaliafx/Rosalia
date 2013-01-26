namespace Rosalia.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using Rosalia.Core.Context;
    using Rosalia.Core.Events;

    public class FailureTask<T> : ITask<T>
    {
        public event EventHandler<TaskMessageEventArgs> MessagePosted;

        public Guid Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<IIdentifiable> IdentifiableChildren { get; private set; }

        public bool HasChildren { get; set; }

        public IEnumerable<ITask<T>> Children { get; set; }

        public ExecutionResult Execute(TaskContext<T> context)
        {
            return new ExecutionResult(ResultType.Failure);
        }
    }
}