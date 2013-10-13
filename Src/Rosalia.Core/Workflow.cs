namespace Rosalia.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Rosalia.Core.Context;
    using Rosalia.Core.Events;
    using Rosalia.Core.Fluent;
    using Rosalia.Core.Tasks.Flow;

    public abstract class Workflow : AbstractSequenceTask, IWorkflow, IExecuter
    {
        private object _data;

        private WorkflowContext _workflowContext;

        public event EventHandler<WorkflowStartEventArgs> WorkflowExecuting;

        public event EventHandler WorkflowExecuted;

        public event EventHandler<TaskMessageEventArgs> WorkflowMessagePosted;

        public event EventHandler<TaskEventArgs> TaskExecuting;

        public event EventHandler<TaskWithResultEventArgs> TaskExecuted;

        public int TasksCount
        {
            get { return SubtasksCount(this) + 1; }
        }

        public virtual ExecutionResult Execute(object inputData)
        {
            _data = inputData;

            OnWorkflowExecuting(CreateContext());

            var executionResult = Execute(this);

            OnWorkflowExecuted();

            return executionResult;
        }

        public virtual void Init(WorkflowContext context)
        {
            _workflowContext = context;

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

        protected virtual void OnTaskMessagePosted(object sender, TaskMessageEventArgs e)
        {
            if (WorkflowMessagePosted != null)
            {
                WorkflowMessagePosted(this, new TaskMessageEventArgs(e.Message, e.Source));
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

//        protected ConditionBuilder If(Predicate<TaskContext> condition)
//        {
//            return new ConditionBuilder(new ForkTask(), condition);
//        }

//        protected RepeaterBuilder<TEnumerableItem> ForEach<TEnumerableItem>(Func<TaskContext, IEnumerable<TEnumerableItem>> enumerableProvider)
//        {
//            return new RepeaterBuilder<TEnumerableItem>(enumerableProvider);
//        }

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
                WorkflowExecuting(this, new WorkflowStartEventArgs(/*_rootTask*/ this));
            }
        }

        private TaskContext CreateContext()
        {
            return new TaskContext(
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