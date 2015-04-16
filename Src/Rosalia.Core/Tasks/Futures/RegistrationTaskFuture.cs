namespace Rosalia.Core.Tasks.Futures
{
    using System.Collections.Generic;
    using Rosalia.Core.Api.Behaviors;

    public class RegistrationTaskFuture<T>: TaskFuture<T> where T : class
    {
        private readonly Identity _identity;
        private readonly IList<ITaskBehavior> _behaviors = new List<ITaskBehavior>();

        public RegistrationTaskFuture(Identity identity)
        {
            _identity = identity;
        }

        public override Identity Identity
        {
            get { return _identity; }
        }

        public override T FetchValue(TaskContext context)
        {
            return context.Results.GetValueOf<T>(_identity);
        }
    }
}