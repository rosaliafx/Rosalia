namespace Rosalia.Core.Events
{
    public class TaskWithResultEventArgs : TaskEventArgs
    {
        public TaskWithResultEventArgs(
            IIdentifiable currentTask, 
            ExecutionResult result) : base(currentTask)
        {
            Result = result;
        }

        public ExecutionResult Result { get; private set; }
    }
}