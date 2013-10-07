namespace Rosalia.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Rosalia.Core.Context;
    using Rosalia.Core.Events;
    using Rosalia.Core.Fluent;
    using Rosalia.Core.Tasks.Flow;

    public abstract class Workflow : IWorkflow, IExecuter
    {
        private object _data;

        private AbstractSequenceTask _rootTask;

        private WorkflowContext _workflowContext;

        public event EventHandler<WorkflowStartEventArgs> WorkflowExecuting;

        public event EventHandler WorkflowExecuted;

        public event EventHandler<TaskMessageEventArgs> MessagePosted;

        public event EventHandler<TaskEventArgs> TaskExecuting;

        public event EventHandler<TaskWithResultEventArgs> TaskExecuted;

        public int TasksCount
        {
            get { return SubtasksCount(_rootTask) + 1; }
        }

        public void Register(Action<ResultBuilder, TaskContext> task, string name = null)
        {
            _rootTask.Register(task, name);
        }

        public void Register<TTask>(
            TTask task, 
            Action<TaskContext, TTask> beforeExecute = null,
            Action<TaskContext, TTask> afterExecute = null, 
            string name = null) where TTask : ITask
        {
            _rootTask.Register(task, beforeExecute, afterExecute, name);
        }

        public virtual ExecutionResult Execute(object inputData)
        {
            _data = inputData;

            OnWorkflowExecuting(CreateContext());

            var executionResult = Execute(_rootTask);

            OnWorkflowExecuted();

            return executionResult;
        }

        public virtual void Init(WorkflowContext context)
        {
            _workflowContext = context;

            _rootTask = new SequenceTask();

            RegisterTasks();
        }

        public ExecutionResult Execute(IExecutable task)
        {
            var dataAware = task as IDataAware;
            if (dataAware != null)
            {
                dataAware.Data = _data;
            }

            OnTaskExecuting(task);

            task.MessagePosted += OnTaskMessagePosted;

            var context = CreateContext();
            var result = task.Execute(context);

            OnTaskExecuted(task, result);

            task.MessagePosted -= OnTaskMessagePosted;

            return result;
        }

        /// <summary>
        /// Registers workflow tasks. Use Name, Task and TaskAction properties.
        /// </summary>
        public abstract void RegisterTasks();

        protected virtual void OnTaskMessagePosted(object sender, TaskMessageEventArgs e)
        {
            if (MessagePosted != null)
            {
                MessagePosted(this, new TaskMessageEventArgs(e.Message, e.Source));
            }
        }

        protected virtual void OnTaskExecuted(IExecutable task, ExecutionResult result)
        {
            if (TaskExecuted != null)
            {
                var args = new TaskWithResultEventArgs(
                    (IIdentifiable)task,
                    result);

                TaskExecuted(this, args);
            }
        }

        protected virtual void OnTaskExecuting(IExecutable task)
        {
            if (TaskExecuting != null)
            {
                var args = new TaskEventArgs(
                    (IIdentifiable)task);

                TaskExecuting(this, args);
            }
        }

        protected SequenceTask Sequence(params ITask[] children)
        {
            return new SequenceTask(children);
        }

        protected ConditionBuilder If(Predicate<TaskContext> condition)
        {
            return new ConditionBuilder(new ForkTask(), condition);
        }

        protected RepeaterBuilder<TEnumerableItem> ForEach<TEnumerableItem>(Func<TaskContext, IEnumerable<TEnumerableItem>> enumerableProvider)
        {
            return new RepeaterBuilder<TEnumerableItem>(enumerableProvider);
        }

        protected virtual void OnWorkflowExecuted()
        {
            if (WorkflowExecuted != null)
            {
                WorkflowExecuted(this, new EventArgs());
            }
        }

        protected virtual void OnWorkflowExecuting(TaskContext context)
        {
            if (WorkflowExecuting != null)
            {
                WorkflowExecuting(this, new WorkflowStartEventArgs(_rootTask));
            }
        }

        private TaskContext CreateContext()
        {
            return new TaskContext(
                /*_inputData, */
                this, 
                _workflowContext.WorkDirectory,
                _workflowContext.Environment);
        }

        private int SubtasksCount(ITask task)
        {
            if (!task.HasChildren)
            {
                return 0;
            }

            return task.Children.Sum(child => 1 + SubtasksCount(child));
        }
    }
}