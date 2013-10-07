namespace Rosalia.Core.Tasks.Flow
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Rosalia.Core.Context;
    using Rosalia.Core.Fluent;

    public class ForkTask : AbstractTask, IEnumerable
    {
        private readonly IList<ChildWithCondition> _children = new List<ChildWithCondition>();

        public override bool HasChildren
        {
            get { return true; }
        }

        public override IEnumerable<ITask> Children
        {
            get { return _children.Select(x => x.Child); }
        }

        public IEnumerator GetEnumerator()
        {
            return Children.GetEnumerator();
        }

        public ConditionBuilder If(Predicate<TaskContext> condition)
        {
            return new ConditionBuilder(this, condition);
        }

        public ForkTask Else()
        {
            if (_children.Count == 0)
            {
                throw new InvalidOperationException("Else method could be called only after a child task was added");
            }

            var lastChild = _children.Last();
            lastChild.BreakOnComplete = true;

            return this;
        }

        public ForkTask Else(ITask child)
        {
            return Else()
                .If(c => true)
                .Then(child);
        }

        public ForkTask Else(Action<ResultBuilder, TaskContext> payload)
        {
            return Else(new SimpleTask(payload));
        }

        public void Add(Predicate<TaskContext> condition, ITask task)
        {
            _children.Add(new ChildWithCondition(task, condition));
        }

        public void Add(Predicate<TaskContext> condition, Action<ResultBuilder, TaskContext> payload)
        {
            _children.Add(new ChildWithCondition(new SimpleTask(payload), condition));
        }

        protected override void Execute(ResultBuilder resultBuilder, TaskContext context)
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
            public ChildWithCondition(ITask child, Predicate<TaskContext> condition, bool breakOnComplete = false)
            {
                BreakOnComplete = breakOnComplete;
                Child = child;
                Condition = condition;
            }

            public ITask Child { get; private set; }

            public Predicate<TaskContext> Condition { get; private set; }

            public bool BreakOnComplete { get; set; }
        }
    }
}