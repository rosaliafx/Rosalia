namespace Rosalia.Runner.Console.Startup.Instantiating
{
    using System;
    using Rosalia.Runner.Console.Startup.Lookup;

    public class ReflectionWorkflowCreator : IWorkflowCreator
    {
        public object CreateWorkflow(WorkflowInfo workflowInfo)
        {
            return Activator.CreateInstance(workflowInfo.WorkflowType);
        }
    }
}