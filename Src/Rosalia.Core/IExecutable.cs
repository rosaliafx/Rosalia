namespace Rosalia.Core
{
    using System;
    using Rosalia.Core.Context;
    using Rosalia.Core.Events;

    public interface IExecutable
    {
        event EventHandler<TaskMessageEventArgs> MessagePosted;

        ExecutionResult Execute(TaskContext context);
    }
}