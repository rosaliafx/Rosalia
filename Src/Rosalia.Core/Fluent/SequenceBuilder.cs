namespace Rosalia.Core.Fluent
{
    using System;
    using System.Collections.Generic;

    public class SequenceBuilder<T>
    {
        private readonly IList<ITask<T>> _children;

        public SequenceBuilder()
        {
            _children = new List<ITask<T>>();
        }

        protected IList<ITask<T>> Children
        {
            get { return _children; }
        }

        public static implicit operator SequenceTask<T>(SequenceBuilder<T> builder)
        {
            return new SequenceTask<T>(builder.Children);
        }

        public SequenceBuilder<T> WithSubtask(ITask<T> child)
        {
            Children.Add(child);
            return this;
        }

        /// <summary>
        /// A shortcut method to add inline simple tasks.
        /// </summary>
        /// <param name="payload">SimpleTask payload</param>
        public SequenceBuilder<T> WithSubtask(Action<ResultBuilder, ExecutionContext<T>> payload)
        {
            Children.Add(new SimpleTask<T>(payload));
            return this;
        }
    }
}