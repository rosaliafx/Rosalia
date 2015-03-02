namespace Rosalia.Core.Tasks
{
    using System.Collections;
    using Rosalia.Core.Engine.Execution;
    using Rosalia.Core.Logging;
    using Rosalia.FileSystem;

    public class TaskContext
    {
        private readonly IResultsStorage _results;
        private readonly IExecutionStrategy _executionStrategy;
        private readonly Identity _identity;
        private readonly ILogRenderer _logRenderer;
        private readonly LogHelper _log;
        private readonly IDirectory _workDirectory;

        public TaskContext(IExecutionStrategy executionStrategy, ILogRenderer logRenderer, IDirectory workDirectory) : this(ResultsStorage.Empty, executionStrategy, null, logRenderer, workDirectory)
        {
        }

        private TaskContext(IResultsStorage results, IExecutionStrategy executionStrategy, Identity identity, ILogRenderer logRenderer, IDirectory workDirectory)
        {
            _identity = identity;
            _results = results;
            _executionStrategy = executionStrategy;
            _logRenderer = logRenderer;
            _workDirectory = workDirectory;
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

        public IDirectory WorkDirectory
        {
            get { return _workDirectory; }
        }

        public TaskContext CreateFor(Identity task)
        {
            return new TaskContext(
                Results,
                ExecutionStrategy,
                task,
                LogRenderer,
                WorkDirectory);
        }

        public TaskContext CreateDerivedFor<T>(Identity task, T value)
        {
            return new TaskContext(
                Results.CreateDerived(task, value),
                ExecutionStrategy,
                Identity,
                LogRenderer,
                WorkDirectory);
        }
    }
}