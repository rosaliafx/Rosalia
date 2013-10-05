namespace Rosalia.Core.Tasks
{
    using System;
    using System.Collections.Generic;
    using Rosalia.Core.Context;
    using Rosalia.Core.Events;
    using Rosalia.Core.Fluent;
    using Rosalia.Core.Logging;

    public abstract class AbstractTask<T> : ITask<T>
    {
        private string _name;

        protected AbstractTask()
        {
            Id = Guid.NewGuid();
        }

        public event EventHandler<TaskMessageEventArgs> MessagePosted;

        public Type[] UnswallowedExceptionTypes { get; set; }

        public Guid Id { get; private set; }
        
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_name))
                {
                    return GetDefaultName();
                }

                return _name;
            }

            set
            {
                _name = value;
            }
        }

        IEnumerable<IIdentifiable> IIdentifiable.IdentifiableChildren
        {
            get
            {
                if (HasChildren)
                {
                    return Children;
                }

                return new IIdentifiable[0];
            }
        }

        public abstract bool HasChildren { get; }
        
        public abstract IEnumerable<ITask<T>> Children { get; }

        public Action<TaskContext<T>> BeforeExecute { get; set; }

        public Action<TaskContext<T>> AfterExecute { get; set; }

        public ExecutionResult Execute(TaskContext<T> context)
        {
            var resultBuilder = new ResultBuilder(PostMessage);

            try
            {
                if (BeforeExecute != null)
                {
                    BeforeExecute(context);
                }

                Execute(resultBuilder, context);

                if (AfterExecute != null)
                {
                    AfterExecute(context);
                }
            }
            catch (Exception ex)
            {
                HandleExecutionException(ex, resultBuilder);
            }

            ExecutionResult result = resultBuilder;

            return result;
        }

        protected void HandleExecutionException(Exception ex, ResultBuilder resultBuilder)
        {
            if (UnswallowedExceptionTypes != null)
            {
                foreach (var type in UnswallowedExceptionTypes)
                {
                    if (type.IsInstanceOfType(ex))
                    {
                        throw ex;
                    }
                }
            }

            resultBuilder.FailWithError(ex);
        }

        protected string GetDefaultName()
        {
            var type = GetType();
            if (type.IsGenericType)
            {
                type = type.GetGenericTypeDefinition();
            }

            return type.Name
                .Replace("Task", string.Empty)
                .Replace(string.Format("`{0}", type.GetGenericArguments().Length), string.Empty);
        }

        protected void PostMessage(Message message)
        {
            if (MessagePosted != null)
            {
                MessagePosted(this, new TaskMessageEventArgs(message, this));
            }
        }

        protected abstract void Execute(ResultBuilder resultBuilder, TaskContext<T> context);
    }
}