namespace Rosalia.Core.Watchers.Logging
{
    using Rosalia.Core.Logging;
    using Rosalia.Core.Watching;

    public class LoggingWatcher : IWorkflowWatcher
    {
        private readonly ILogRenderer _renderer;

        public LoggingWatcher(ILogRenderer renderer)
        {
            _renderer = renderer;
        }

        public void Register(IWorkflowEventsAware eventsAware)
        {
            var level = 0;

            eventsAware.TaskExecuting += (sender, args) =>
            {
                _renderer.AppendMessage(level, string.Format("Start executing task <{0}>", args.CurrentTask.Name), MessageLevel.Info, MessageType.TaskStart);
                level++;
            };

            eventsAware.TaskExecuted += (sender, args) =>
            {
                level--;

                _renderer.AppendMessage(
                    level, 
                    string.Format("Task <{0}> executed", args.CurrentTask.Name), 
                    args.Result.ResultType == ResultType.Success ? MessageLevel.Success : MessageLevel.Error, 
                    MessageType.TaskStart);
            };

            eventsAware.MessagePosted += (sender, args) =>
            {
                _renderer.AppendMessage(level, args.Message.Text, args.Message.Level, MessageType.TaskMessage);
            };
        }
    }
}