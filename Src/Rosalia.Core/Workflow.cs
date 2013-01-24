namespace Rosalia.Core
{
    using System;
    using Rosalia.Core.Context;
    using Rosalia.Core.Events;
    using Rosalia.Core.Fluent;
    using Rosalia.Core.Logging;
    using Rosalia.Core.Result;

    public abstract class Workflow<TData> : IGenericWorkflow<TData>, IExecuter<TData>, ILogger
    {
        private TData _inputData;

        private ITask<TData> _rootTask;

        private IExecutable<TData> _currentTask;

        private int _level;

        private WorkflowContext _workflowContext;

        public event EventHandler<WorkflowStartEventArgs> WorkflowStart;

        public event EventHandler WorkflowComplete;

        public event EventHandler<LogMessageEventArgs> LogMessagePost;

        public event EventHandler<TaskEventArgs> TaskStartExecution;

        public event EventHandler<TaskWithResultEventArgs> TaskCompleteExecution;

        public int TasksCount
        {
            get { return SubtasksCount(_rootTask) + 1; }
        }

        protected SequenceTask<TData> Sequence(params ITask<TData>[] children)
        {
            return new SequenceTask<TData>(children);
        }

        public ExecutionResult Execute(object inputData)
        {
            if (inputData is TData)
            {
                return ((IGenericWorkflow<TData>)this).Execute((TData) inputData);
            }

            throw new Exception("Wrong workflow context");
        }

        public virtual void Init(WorkflowContext context)
        {
            _workflowContext = context;
            _rootTask = RootTask;
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

        protected virtual void OnBeforeExecute(TaskContext<TData> context)
        {
            if (WorkflowStart != null)
            {
                WorkflowStart(this, new WorkflowStartEventArgs(_rootTask));
            }
        }

        public abstract ITask<TData> RootTask { get; }

        public ExecutionResult Execute(IExecutable<TData> task)
        {
            if (TaskStartExecution != null)
            {
                var args = new TaskEventArgs(
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

        protected ITask<TData> Task(Action<ResultBuilder, TaskContext<TData>> payload)
        {
            return new SimpleTask<TData>(payload);
        }

        private TaskContext<TData> CreateContext()
        {
            return new TaskContext<TData>(
                _inputData, 
                this, 
                this, 
                _workflowContext.WorkDirectory,
                _workflowContext.Environment);
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