namespace Rosalia.Core.Api
{
    using Rosalia.FileSystem;

    public abstract class Workflow : TaskRegistry, IWorkflow
    {
        public IDirectory WorkDirectory { get; internal set; }
    }
}