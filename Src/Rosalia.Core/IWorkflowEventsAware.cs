namespace Rosalia.Core
{
    using System;
    using Rosalia.Core.Events;

    public interface IWorkflowEventsAware
    {
        event EventHandler<WorkflowStartEventArgs> WorkflowExecuting;

        event EventHandler WorkflowExecuted;

        event EventHandler<LogMessageEventArgs> LogMessagePost;

        event EventHandler<TaskEventArgs> TaskExecuting;

        event EventHandler<TaskWithResultEventArgs> TaskExecuted;
    }
}