namespace Rosalia.Core.Tasks.Flow
{
    using System;
    using System.Collections.Generic;
    using Rosalia.Core.Context;
    using Rosalia.Core.Fluent;

    public abstract class AbstractSequenceTask : AbstractTask, IDataAware
    {
        private readonly IList<ITask> _children;
        
        /// <summary>
        /// This action will be used for task registration (if set).
        /// </summary>
        private IList<ITask> _taskDestination;

        protected AbstractSequenceTask() : this(new List<ITask>())
        {
        }

        protected AbstractSequenceTask(params ITask[] children) : this(new List<ITask>(children))
        {
        }

        protected AbstractSequenceTask(IList<ITask> children)
        {
            _children = children;
        }

        public object Data { get; set; }

        protected TaskContext Context { get; private set; }

        protected ResultBuilder Result { get; private set; }

        public override bool HasChildren
        {
            get { return true; }
        }

        public override IEnumerable<ITask> Children
        {
            get { return _children; }
        }

        public AbstractSequenceTask Register(
            Action task,
            string name = null)
        {
            Action<ResultBuilder, TaskContext> payload = (result, context) =>
            {
                Result = result;
                task.Invoke();
            };

            RegisterTask(name, new SimpleTask(payload), null, null);
            return this;
        }

        public AbstractSequenceTask Register<TTask>(
            TTask task) where TTask : ITask
        {
            RegisterTask(null, task, null, null);

            return this;
        }

        public AbstractSequenceTask Register<TTask>(
            TTask task,
            Action<TTask> beforeExecute = null,
            Action<TTask> afterExecute = null,
            string name = null) where TTask : ITask
        {
            RegisterTask(name, task, beforeExecute, afterExecute);

            return this;
        }

        public RepeatTask<TItem> ForEach<TItem>(Func<IEnumerable<TItem>> items, Action<TItem> tasksRegistrationAction)
        {
            return new RepeatTask<TItem>()
                .ForEach(context => items.Invoke())
                .Do((context, item) =>
                {
                    var tasks = new List<ITask>();
                    _taskDestination = tasks;

                    tasksRegistrationAction.Invoke(item);

                    _taskDestination = null;

                    if (tasks.Count == 0)
                    {
                        throw new Exception("No task was registered for repeater body");
                    }

                    if (tasks.Count == 1)
                    {
                        return tasks[0];
                    }

                    return new SequenceTask(tasks);
                });
        }

        protected override void Execute(ResultBuilder resultBuilder, TaskContext context)
        {
            Context = context;

            foreach (var child in Children)
            {
                var childResult = context.Executer.Execute(child);
                if (childResult.ResultType != ResultType.Success)
                {
                    resultBuilder.Fail();
                    return;
                }
            }
        }

        public abstract void RegisterTasks();

        private void RegisterTask<TTask>(
            string name,
            TTask task,
            Action<TTask> beforeExecute,
            Action<TTask> afterExecute) where TTask : ITask
        {
            if (beforeExecute != null)
            {
                task.BeforeExecute = (context, result) =>
                {
                    Result = result;
                    beforeExecute.Invoke(task);
                };
            }

            if (afterExecute != null)
            {
                task.AfterExecute = context => afterExecute.Invoke(task);
            }

            if (name != null)
            {
                task.Name = name;
            }

            if (_taskDestination == null)
            {
                _children.Add(task);    
            } 
            else
            {
                _taskDestination.Add(task);
            }
        }
    }
}