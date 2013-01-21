namespace Rosalia.Core.Events
{
    using System;
    using Rosalia.Core.Logging;

    public class TaskEventArgs : EventArgs
    {
        public TaskEventArgs(IIdentifiable currentTask, IIdentifiable parentTask, ILogger logger, int currentTaskDepth)
        {
            CurrentTaskDepth = currentTaskDepth;
            ParentTask = parentTask;
            Logger = logger;
            CurrentTask = currentTask;
        }

        public IIdentifiable ParentTask { get; private set; }

        public IIdentifiable CurrentTask { get; private set; }

        public int CurrentTaskDepth { get; private set; }

        public ILogger Logger { get; private set; }
    }
}