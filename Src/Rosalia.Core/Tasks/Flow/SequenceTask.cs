namespace Rosalia.Core.Tasks.Flow
{
    using System;
    using System.Collections.Generic;
    using Rosalia.Core.Context;
    using Rosalia.Core.Fluent;

    public class SequenceTask<T> : AbstractSequenceTask<T>
    {
        public SequenceTask()
        {
        }

        public SequenceTask(params ITask<T>[] children) : base(children)
        {
        }

        public SequenceTask(IList<ITask<T>> children) : base(children)
        {
        }

        public SequenceTask<T> WithSubtask(ITask<T> child)
        {
            Register(child);
            return this;
        }

        /// <summary>
        /// A shortcut method to add a <code>SimpleTask</code> as a child.
        /// </summary>
        /// <param name="child"><code>SimpleTask</code> payload action</param>
        public SequenceTask<T> WithSubtask(Action<ResultBuilder, TaskContext<T>> child)
        {
            Register(new SimpleTask<T>(child));
            return this;
        }

        public override void RegisterTasks()
        {
        }
    }
}