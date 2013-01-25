namespace Rosalia.Core.Tasks
{
    using System;
    using System.Collections.Generic;
    using Rosalia.Core.Context;
    using Rosalia.Core.Fluent;
    using Rosalia.Core.Result;

    public abstract class AbstractTask<T> : ITask<T>
    {
        private string _name;

        protected AbstractTask()
        {
            Id = Guid.NewGuid();
        }

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

            protected set
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

        public ExecutionResult Execute(TaskContext<T> context)
        {
            var resultBuilder = new ResultBuilder();

            try
            {
                Execute(resultBuilder, context);
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

        protected abstract void Execute(ResultBuilder resultBuilder, TaskContext<T> context);
    }
}