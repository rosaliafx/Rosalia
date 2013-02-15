﻿namespace Rosalia.Core
{
    using Rosalia.Core.Context;

    public interface IWorkflow : IWorkflowEventsAware
    {
        int TasksCount { get; }

        ExecutionResult Execute(object inputData);

        /// <summary>
        /// Initializes the workflow, creates tasks graph.
        /// </summary>
        void Init(WorkflowContext context); 
    }
}