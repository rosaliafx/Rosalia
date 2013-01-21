namespace Rosalia.Core
{
    using System;
    using System.Collections.Generic;
    using Rosalia.Core.Fluent;

    public class SequenceTask<T> : AbstractTask<T>
    {
        private readonly IList<ITask<T>> _children;

        public SequenceTask() : this(new List<ITask<T>>())
        {
        }

        public SequenceTask(params ITask<T>[] children) : this(new List<ITask<T>>(children))
        {
        }

        public SequenceTask(IList<ITask<T>> children)
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

        public SequenceTask<T> WithSubtask(ITask<T> child)
        {
            _children.Add(child);
            return this;
        }

        /// <summary>
        /// A shortcut method to add a <code>SimpleTask</code> as a child.
        /// </summary>
        /// <param name="child"><code>SimpleTask</code> payload action</param>
        public SequenceTask<T> WithSubtask(Action<ResultBuilder, ExecutionContext<T>> child)
        {
            _children.Add(new SimpleTask<T>(child));
            return this;
        }

        protected override void Execute(ResultBuilder resultBuilder, ExecutionContext<T> context)
        {
            foreach (var child in _children)
            {
                var childResult = context.Executer.Execute(child);
                if (childResult.ResultType != ResultType.Success)
                {
                    resultBuilder.Fail();
                    return;
                }
            }
        }
    }
}