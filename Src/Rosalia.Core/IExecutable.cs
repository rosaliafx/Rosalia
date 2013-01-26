namespace Rosalia.Core
{
    using System;
    using Rosalia.Core.Context;
    using Rosalia.Core.Events;

    public interface IExecutable<TContext>
    {
        event EventHandler<TaskMessageEventArgs> MessagePosted;

        ExecutionResult Execute(TaskContext<TContext> context);
    }
}