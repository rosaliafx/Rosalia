namespace Rosalia.Core
{
    using System;
    using System.Linq;
    using Rosalia.Core.Context;
    using Rosalia.Core.Events;
    using Rosalia.Core.Fluent;
    using Rosalia.Core.Logging;
    using Rosalia.Core.Result;
    using Rosalia.Core.Tasks;
    using Rosalia.Core.Tasks.Flow;

    public abstract class Workflow<TData> : IGenericWorkflow<TData>, IExecuter<TData>, ILogger
    {
        private TData _inputData;

        private ITask<TData> _rootTask;

        private IExecutable<TData> _currentTask;

        private int _level;

        private WorkflowContext _workflowContext;

        public event EventHandler<WorkflowStartEventArgs> WorkflowExecuting;

        public event EventHandler WorkflowExecuted;

        public event EventHandler<LogMessageEventArgs> LogMessagePost;

        public event EventHandler<TaskEventArgs> TaskExecuting;

        public event EventHandler<TaskWithResultEventArgs> TaskExecuted;

        public int TasksCount
        {
            get { return SubtasksCount(_rootTask) + 1; }
        }

        public abstract ITask<TData> RootTask { get; }

        public ExecutionResult Execute(object inputData)
        {
            if (inputData is TData)
            {
                return ((IGenericWorkflow<TData>)this).Execute((TData)inputData);
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

            OnWorkflowExecuting(CreateContext());

            var executionResult = Execute(_rootTask);

            OnWorkflowExecuted();

            return executionResult;
        }

        public ExecutionResult Execute(IExecutable<TData> task)
        {
            OnTaskExecuting(task);

            var initialCurrentTask = _currentTask;

            _currentTask = task;
            _level++;

            var context = CreateContext();
            var result = task.Execute(context);

            _currentTask = initialCurrentTask;
            _level--;

            OnTaskExecuted(task, result);

            return result;
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

        protected virtual void OnTaskExecuted(IExecutable<TData> task, ExecutionResult result)
        {
            if (TaskExecuted != null)
            {
                var args = new TaskWithResultEventArgs(
                    (IIdentifiable)task,
                    (IIdentifiable)_currentTask,
                    this,
                    _level,
                    result);

                TaskExecuted(this, args);
            }
        }

        protected virtual void OnTaskExecuting(IExecutable<TData> task)
        {
            if (TaskExecuting != null)
            {
                var args = new TaskEventArgs(
                    (IIdentifiable)task,
                    (IIdentifiable)_currentTask,
                    this,
                    _level);

                TaskExecuting(this, args);
            }
        }

        protected SequenceTask<TData> Sequence(params ITask<TData>[] children)
        {
            return new SequenceTask<TData>(children);
        }

        protected virtual void OnWorkflowExecuted()
        {
            if (WorkflowExecuted != null)
            {
                WorkflowExecuted(this, new EventArgs());
            }
        }

        protected virtual void OnWorkflowExecuting(TaskContext<TData> context)
        {
            if (WorkflowExecuting != null)
            {
                WorkflowExecuting(this, new WorkflowStartEventArgs(_rootTask));
            }
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

        private int SubtasksCount(ITask<TData> task)
        {
            if (!task.HasChildren)
            {
                return 0;
            }

            return task.Children.Sum(child => 1 + SubtasksCount(child));
        }
    }
}