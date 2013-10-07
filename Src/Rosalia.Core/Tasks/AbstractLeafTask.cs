namespace Rosalia.Core.Tasks
{
    using System;
    using System.Collections.Generic;

    public abstract class AbstractLeafTask : AbstractTask
    {
        public override bool HasChildren
        {
            get { return false; }
        }

        public override IEnumerable<ITask> Children
        {
            get { throw new InvalidOperationException(); }
        }
    }
}