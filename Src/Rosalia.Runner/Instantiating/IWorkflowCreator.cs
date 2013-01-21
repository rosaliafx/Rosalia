namespace Rosalia.Runner.Instantiating
{
    using Rosalia.Runner.Lookup;

    public interface IWorkflowCreator
    {
        object CreateWorkflow(WorkflowInfo workflowInfo);
    }
}