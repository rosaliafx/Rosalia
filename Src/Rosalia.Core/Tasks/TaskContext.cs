namespace Rosalia.Core.Tasks
{
    using Rosalia.Core.Engine.Execution;
    using Rosalia.Core.Logging;

    public class TaskContext
    {
        private readonly IResultsStorage _results;
        private readonly IExecutionStrategy _executionStrategy;
        private readonly Identity _identity;
        private readonly ILogRenderer _logRenderer;
        private readonly LogHelper _log;

        public TaskContext(IExecutionStrategy executionStrategy, ILogRenderer logRenderer) : this(ResultsStorage.Empty, executionStrategy, null, logRenderer)
        {
        }

        private TaskContext(IResultsStorage results, IExecutionStrategy executionStrategy, Identity identity, ILogRenderer logRenderer)
        {
            _identity = identity;
            _results = results;
            _executionStrategy = executionStrategy;
            _logRenderer = logRenderer;
            _log = new LogHelper(message => logRenderer.Render(message, Identity));
        }

        public IResultsStorage Results
        {
            get { return _results; }
        }

        public IExecutionStrategy ExecutionStrategy
        {
            get { return _executionStrategy; }
        }

        public LogHelper Log
        {
            get { return _log; }
        }

        public ILogRenderer LogRenderer
        {
            get { return _logRenderer; }
        }

        public Identity Identity
        {
            get { return _identity; }
        }

        public TaskContext CreateFor(Identity task)
        {
            return new TaskContext(
                Results,
                ExecutionStrategy,
                task,
                LogRenderer);
        }

        public TaskContext CreateDerivedFor<T>(Identity task, T value)
        {
            return new TaskContext(
                Results.CreateDerived(task, value),
                ExecutionStrategy,
                Identity,
                LogRenderer);
        }
    }
}