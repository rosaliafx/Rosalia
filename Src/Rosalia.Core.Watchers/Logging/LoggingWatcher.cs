namespace Rosalia.Core.Watchers.Logging
{
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

            eventsAware.TaskStartExecution += (sender, args) =>
            {
                level = args.CurrentTaskDepth;
            };
            eventsAware.TaskCompleteExecution += (sender, args) =>
            {
                level = args.CurrentTaskDepth;
            };

            eventsAware.LogMessagePost += (sender, args) =>
            {
                if (args.Template == null)
                {
                    args.Template = string.Empty;
                }

                var formattedMessage = args.Args.Length > 0 ?
                    string.Format(args.Template, args.Args) :
                    args.Template;

                _renderer.AppendMessage(level, formattedMessage, args.Level);
            };
        }
    }
}