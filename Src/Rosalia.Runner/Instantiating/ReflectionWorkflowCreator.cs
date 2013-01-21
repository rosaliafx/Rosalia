namespace Rosalia.Runner.Instantiating
{
    using System;
    using Rosalia.Runner.Lookup;

    public class ReflectionWorkflowCreator : IWorkflowCreator
    {
        public object CreateWorkflow(WorkflowInfo workflowInfo)
        {
            return Activator.CreateInstance(workflowInfo.WorkflowType);
        }
    }
}