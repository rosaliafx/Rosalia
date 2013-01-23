namespace Rosalia.Core.Context
{
    using Rosalia.Core.Context.Environment;
    using Rosalia.Core.FileSystem;

    /// <summary>
    /// Workflow execution context.
    /// </summary>
    public class WorkflowContext
    {
        public WorkflowContext(IEnvironment environment, IDirectory workDirectory)
        {
            Environment = environment;
            WorkDirectory = workDirectory;
        }

        public IEnvironment Environment { get; private set; }

        public IDirectory WorkDirectory { get; private set; }
    }
}