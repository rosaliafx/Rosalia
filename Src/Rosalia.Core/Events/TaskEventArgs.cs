namespace Rosalia.Core.Events
{
    using System;

    public class TaskEventArgs : EventArgs
    {
        public TaskEventArgs(IIdentifiable currentTask)
        {
            CurrentTask = currentTask;
        }

        public IIdentifiable CurrentTask { get; private set; }
    }
}