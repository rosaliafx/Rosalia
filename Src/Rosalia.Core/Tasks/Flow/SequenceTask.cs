namespace Rosalia.Core.Tasks.Flow
{
    using System.Collections.Generic;

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

        public override void RegisterTasks()
        {
        }
    }
}