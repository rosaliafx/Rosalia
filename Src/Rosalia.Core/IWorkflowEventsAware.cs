namespace Rosalia.Core
{
    using System;
    using Rosalia.Core.Events;

    public interface IWorkflowEventsAware
    {
        event EventHandler<WorkflowStartEventArgs> WorkflowStart;

        event EventHandler WorkflowComplete;

        event EventHandler<LogMessageEventArgs> LogMessagePost;

        event EventHandler<TaskEventArgs> TaskStartExecution;

        event EventHandler<TaskWithResultEventArgs> TaskCompleteExecution;
    }
}