namespace Rosalia.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Rosalia.Core.Fluent;

    public class ForkTask<T> : AbstractTask<T>, IEnumerable
    {
        private readonly IList<ChildWithCondition> _children = new List<ChildWithCondition>();

        public override bool HasChildren
        {
            get { return true; }
        }

        public override IEnumerable<ITask<T>> Children
        {
            get { return _children.Select(x => x.Child); }
        }

        public IEnumerator GetEnumerator()
        {
            return Children.GetEnumerator();
        }

        public ConditionBuilder<T> If(Predicate<ExecutionContext<T>> condition)
        {
            return new ConditionBuilder<T>(this, condition);
        }

        public ForkTask<T> Else()
        {
            if (_children.Count == 0)
            {
                throw new InvalidOperationException("Else method could be called only after a child task was added");
            }

            var lastChild = _children.Last();
            lastChild.BreakOnComplete = true;

            return this;
        }

        public ForkTask<T> Else(ITask<T> child)
        {
            return Else()
                .If(c => true)
                .Then(child);
        }

        public ForkTask<T> Else(Action<ResultBuilder, ExecutionContext<T>> payload)
        {
            return Else(new SimpleTask<T>(payload));
        }

        public void Add(Predicate<ExecutionContext<T>> condition, ITask<T> task)
        {
            _children.Add(new ChildWithCondition(task, condition));
        }

        public void Add(Predicate<ExecutionContext<T>> condition, Action<ResultBuilder, ExecutionContext<T>> payload)
        {
            _children.Add(new ChildWithCondition(new SimpleTask<T>(payload), condition));
        }

        protected override void Execute(ResultBuilder resultBuilder, ExecutionContext<T> context)
        {
            foreach (var childWithCondition in _children)
            {
                if (childWithCondition.Condition(context))
                {
                    var childResult = context.Executer.Execute(childWithCondition.Child);
                    if (childResult.ResultType != ResultType.Success)
                    {
                        resultBuilder.Fail();
                        return;
                    }

                    if (childWithCondition.BreakOnComplete)
                    {
                        return;
                    }
                }
            }
        }

        internal class ChildWithCondition
        {
            public ChildWithCondition(ITask<T> child, Predicate<ExecutionContext<T>> condition, bool breakOnComplete = false)
            {
                BreakOnComplete = breakOnComplete;
                Child = child;
                Condition = condition;
            }

            public ITask<T> Child { get; private set; }

            public Predicate<ExecutionContext<T>> Condition { get; private set; }

            public bool BreakOnComplete { get; set; }
        }
    }
}