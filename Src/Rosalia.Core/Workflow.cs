namespace Rosalia.Core
{
    using System;
    using Rosalia.Core.Events;
    using Rosalia.Core.FileSystem;
    using Rosalia.Core.Logging;
    using Rosalia.Core.Result;

    public abstract class Workflow<TData> : IGenericWorkflow<TData>, IExecuter<TData>, ILogger
    {
        private TData _inputData;

        private ITask<TData> _rootTask;

        private IExecutable<TData> _currentTask;

        private int _level;

        public event EventHandler<WorkflowStartEventArgs> WorkflowStart;

        public event EventHandler WorkflowComplete;

        public event EventHandler<LogMessageEventArgs> LogMessagePost;

        public event EventHandler<TaskEventArgs> TaskStartExecution;

        public event EventHandler<TaskWithResultEventArgs> TaskCompleteExecution;

        public IDirectory WorkDirectory { get; set; }

        public int TasksCount
        {
            get { return SubtasksCount(_rootTask) + 1; }
        }

        protected SequenceTask<TData> Sequence
        {
            get { return new SequenceTask<TData>(); }
        }

        public ExecutionResult Execute(object inputData)
        {
            if (inputData is TData)
            {
                return ((IGenericWorkflow<TData>)this).Execute((TData) inputData);
            }

            throw new Exception("Wrong workflow context");
        }

        public virtual void Init()
        {
            _rootTask = CreateRootTask();
        }

        public ExecutionResult Execute(TData inputData)
        {
            _level = 0;
            _inputData = inputData;

            OnBeforeExecute(CreateContext());

            var executionResult = Execute(_rootTask);

            OnWorkflowComplete();

            return executionResult;
        }

        protected virtual void OnWorkflowComplete()
        {
            if (WorkflowComplete != null)
            {
                WorkflowComplete(this, new EventArgs());
            }
        }

        protected virtual void OnBeforeExecute(ExecutionContext<TData> context)
        {
            if (WorkflowStart != null)
            {
                WorkflowStart(this, new WorkflowStartEventArgs(_rootTask));
            }
        }

        public abstract ITask<TData> CreateRootTask();

        public ExecutionResult Execute(IExecutable<TData> task)
        {
            if (TaskStartExecution != null)
            {
                TaskEventArgs args = new TaskEventArgs(
                    (IIdentifiable)task,
                    (IIdentifiable)_currentTask,
                    this,
                    _level);

                TaskStartExecution(this, args);
            }

            var initialCurrentTask = _currentTask;

            _currentTask = task;
            _level++;

            var context = CreateContext();
            var result = task.Execute(context);

            _currentTask = initialCurrentTask;
            _level--;

            if (TaskCompleteExecution != null)
            {
                var args = new TaskWithResultEventArgs(
                    (IIdentifiable)task,
                    (IIdentifiable)_currentTask,
                    this,
                    _level,
                    result);

                TaskCompleteExecution(this, args);
            }

            return result;
        }

        private ExecutionContext<TData> CreateContext()
        {
            return new ExecutionContext<TData>(_inputData, this, this, WorkDirectory);
        }

        public void Log(MessageLevel level, string message, params object[] args)
        {
            if (LogMessagePost != null)
            {
                var eventArgs = new LogMessageEventArgs
                {
                    Template = message,
                    Level = level,
                    Args = args
                };

                LogMessagePost(this, eventArgs);
            }
        }

        private int SubtasksCount(ITask<TData> task)
        {
            if (!task.HasChildren)
            {
                return 0;
            }

            int count = 0;
            foreach (var child in task.Children)
            {
                count += 1 + SubtasksCount(child);
            }

            return count;
        }
    }
}