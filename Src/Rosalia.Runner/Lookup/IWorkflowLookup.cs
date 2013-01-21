namespace Rosalia.Runner.Lookup
{
    using System.Collections.Generic;

    public interface IWorkflowLookup
    {
        bool CanHandle(LookupOptions options);

        IEnumerable<WorkflowInfo> Find(LookupOptions options);
    }
}