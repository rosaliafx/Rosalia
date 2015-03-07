namespace Rosalia.Runner.Console.Startup.Instantiating
{
    using Rosalia.Runner.Console.Startup.Lookup;

    public interface IWorkflowCreator
    {
        object CreateWorkflow(WorkflowInfo workflowInfo);
    }
}