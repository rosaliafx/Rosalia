namespace Rosalia.TestingSupport.Helpers
{
    using System;
    using Rosalia.Core.Api;
    using Rosalia.Core.Tasks.Futures;

    public class QuickSubflow<T> : TaskRegistry<T> where T : class
    {
        private readonly Func<TaskRegistry<T>, ITaskFuture<T>> _defineAction;

        public QuickSubflow(Func<TaskRegistry<T>, ITaskFuture<T>> defineAction)
        {
            _defineAction = defineAction;
        }

        protected override ITaskFuture<T> RegisterTasks()
        {
            return _defineAction.Invoke(this);
        }

        public static QuickSubflow<T> Define(Func<TaskRegistry<T>, ITaskFuture<T>> defineAction)
        {
            return new QuickSubflow<T>(defineAction);
        }
    }
}