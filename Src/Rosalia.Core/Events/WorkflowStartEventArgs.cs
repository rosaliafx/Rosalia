namespace Rosalia.Core.Events
{
    using System;

    public class WorkflowStartEventArgs : EventArgs
    {
        public WorkflowStartEventArgs(IIdentifiable rootTask)
        {
            RootTask = rootTask;
        }

        public IIdentifiable RootTask { get; private set; }
    }
}