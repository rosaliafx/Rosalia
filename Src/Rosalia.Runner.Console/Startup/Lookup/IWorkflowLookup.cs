namespace Rosalia.Runner.Console.Startup.Lookup
{
    using System.Collections.Generic;

    public interface IWorkflowLookup
    {
        bool CanHandle(LookupOptions options);

        IEnumerable<WorkflowInfo> Find(LookupOptions options);
    }
}