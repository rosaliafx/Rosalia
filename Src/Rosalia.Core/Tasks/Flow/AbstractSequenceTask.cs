namespace Rosalia.Core.Tasks.Flow
{
    using System;
    using System.Collections.Generic;
    using Rosalia.Core.Context;
    using Rosalia.Core.Fluent;

    public abstract class AbstractSequenceTask<T> : AbstractTask<T>
    {
        private readonly IList<ITask<T>> _children;

        protected AbstractSequenceTask() : this(new List<ITask<T>>())
        {
        }

        protected AbstractSequenceTask(params ITask<T>[] children) : this(new List<ITask<T>>(children))
        {
        }

        protected AbstractSequenceTask(IList<ITask<T>> children)
        {
            _children = children;
        }

        public override bool HasChildren
        {
            get { return true; }
        }

        public override IEnumerable<ITask<T>> Children
        {
            get { return _children; }
        }

        public AbstractSequenceTask<T> Register(
            Action<ResultBuilder, TaskContext<T>> task,
            string name = null)
        {
            return Register(new SimpleTask<T>(task), null, null, name);
        }

        public AbstractSequenceTask<T> Register<TTask>(
            TTask task,
            Action<TaskContext<T>, TTask> beforeExecute = null,
            Action<TaskContext<T>, TTask> afterExecute = null,
            string name = null) where TTask : ITask<T>
        {
            if (beforeExecute != null)
            {
                task.BeforeExecute = context => beforeExecute.Invoke(context, task);
            }

            if (afterExecute != null)
            {
                task.AfterExecute = context => afterExecute.Invoke(context, task);
            }

            _children.Add(task);

            return this;
        }

        protected override void Execute(ResultBuilder resultBuilder, TaskContext<T> context)
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