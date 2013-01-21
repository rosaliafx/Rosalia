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
            int level = 0;

//            eventsAware.WorkflowStart += (sender, args) => _renderer.Init();
//            eventsAware.WorkflowComplete += (sender, args) => _renderer.Dispose();

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
                var formattedMessage = args.Args.Length > 0 ?
                    string.Format(args.Template, args.Args) :
                    args.Template;

                _renderer.AppendMessage(level, formattedMessage, args.Level);
            };
        }
    }
}