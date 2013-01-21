namespace Rosalia.Runner.Instantiating
{
    using Rosalia.Runner.Lookup;

    public interface IContextCreator
    {
        object CreateContext(WorkflowInfo workflowInfo);
    }
}