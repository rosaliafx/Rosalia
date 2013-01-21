namespace Rosalia.Core
{
    using System.Collections.Generic;
    using Rosalia.Core.Fluent;

    public class SequenceTask<T> : AbstractTask<T>
    {
        private readonly IList<ITask<T>> _children;

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