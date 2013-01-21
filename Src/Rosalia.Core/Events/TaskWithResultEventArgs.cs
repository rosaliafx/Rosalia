namespace Rosalia.Core.Events
{
    using Rosalia.Core.Logging;
    using Rosalia.Core.Result;

    public class TaskWithResultEventArgs : TaskEventArgs
    {
        public TaskWithResultEventArgs(
            IIdentifiable currentTask, 
            IIdentifiable parentTask, 
            ILogger logger, 
            int currentTaskDepth, 
            ExecutionResult result) : base(currentTask, parentTask, logger, currentTaskDepth)
        {
            Result = result;
        }

        public ExecutionResult Result { get; private set; }
    }
}