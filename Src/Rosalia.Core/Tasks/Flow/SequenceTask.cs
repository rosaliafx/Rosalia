namespace Rosalia.Core.Tasks.Flow
{
    using System;
    using System.Collections.Generic;
    using Rosalia.Core.Context;
    using Rosalia.Core.Fluent;

    public class SequenceTask : AbstractSequenceTask
    {
        public SequenceTask()
        {
        }

        public SequenceTask(params ITask[] children) : base(children)
        {
        }

        public SequenceTask(IList<ITask> children) : base(children)
        {
        }

        public SequenceTask WithSubtask(ITask child)
        {
            Register(child);
            return this;
        }

        /// <summary>
        /// A shortcut method to add a <code>SimpleTask</code> as a child.
        /// </summary>
        /// <param name="child"><code>SimpleTask</code> payload action</param>
        public SequenceTask WithSubtask(Action<ResultBuilder, TaskContext> child)
        {
            Register(new SimpleTask(child));
            return this;
        }

        public override void RegisterTasks()
        {
        }
    }
}