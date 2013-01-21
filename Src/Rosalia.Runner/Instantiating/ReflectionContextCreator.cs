namespace Rosalia.Runner.Instantiating
{
    using System;
    using Rosalia.Runner.Lookup;

    public class ReflectionContextCreator : IContextCreator
    {
        public object CreateContext(WorkflowInfo workflowInfo)
        {
            return Activator.CreateInstance(workflowInfo.ContextType);
        }
    }
}