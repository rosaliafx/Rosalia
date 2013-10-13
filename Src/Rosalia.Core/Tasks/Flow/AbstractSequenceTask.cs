namespace Rosalia.Core.Tasks.Flow
{
    using System;
    using System.Collections.Generic;
    using Rosalia.Core.Context;
    using Rosalia.Core.Fluent;

    public abstract class AbstractSequenceTask : AbstractTask, IDataAware
    {
        private readonly IList<ITask> _children;

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

        public override bool HasChildren
        {
            get { return true; }
        }

        public override IEnumerable<ITask> Children
        {
            get { return _children; }
        }

        public AbstractSequenceTask Register(
            Action<ResultBuilder, TaskContext> task,
            string name = null)
        {
            return Register(new SimpleTask(task), null, null, name);
        }

        public AbstractSequenceTask Register<TTask>(
            TTask task,
            Action<TaskContext, TTask> beforeExecute = null,
            Action<TaskContext, TTask> afterExecute = null,
            string name = null) where TTask : ITask
        {
            if (beforeExecute != null)
            {
                task.BeforeExecute = (context, result) => beforeExecute.Invoke(context, task);
            }

            if (afterExecute != null)
            {
                task.AfterExecute = context => afterExecute.Invoke(context, task);
            }

            if (name != null)
            {
                task.Name = name;
            }

            _children.Add(task);

            return this;
        }

        protected override void Execute(ResultBuilder resultBuilder, TaskContext context)
        {
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
    }
}