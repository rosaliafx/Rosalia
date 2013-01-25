namespace Rosalia.Core.Watchers.Logging
{
    using System.Collections.Generic;
    using System.Linq;
    using Rosalia.Core.Logging;
    using Rosalia.Core.Result;
    using Rosalia.Core.Watching;

    public class LogTaskStartWatcher : IWorkflowWatcher
    {
        public void Register(IWorkflowEventsAware eventsAware)
        {
            eventsAware.TaskExecuting += (sender, args) => 
                args.Logger.Info(
                "Start executing task {0}", 
                args.CurrentTask.Name);

            eventsAware.TaskExecuted += (sender, args) =>
            {
                args.Logger.Log(
                    args.Result.ResultType == ResultType.Success
                        ? MessageLevel.Success
                        : MessageLevel.Error,
                    "Task {0} complete",
                    args.CurrentTask.Name);

                LogMessages(args.Result.Messages, args.Logger);
            };
        }

        private void LogMessages(IEnumerable<ResultMessage> messages, ILogger logger)
        {
            if (messages == null)
            {
                return;
            }

            var messagesList = messages.ToList();
            if (messagesList.Count == 0)
            {
                return;
            }

            logger.Log(MessageLevel.Info, "------------ Task messages: ------------");
            foreach (var message in messagesList)
            {
                logger.Log(message.Level, message.Text);
            }

            logger.Log(MessageLevel.Info, "----------------------------------------");
        }
    }
}