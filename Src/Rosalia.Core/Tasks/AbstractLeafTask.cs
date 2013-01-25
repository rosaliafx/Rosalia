namespace Rosalia.Core.Tasks
{
    using System;
    using System.Collections.Generic;

    public abstract class AbstractLeafTask<T> : AbstractTask<T>
    {
        public override bool HasChildren
        {
            get { return false; }
        }

        public override IEnumerable<ITask<T>> Children
        {
            get { throw new InvalidOperationException(); }
        }
    }
}