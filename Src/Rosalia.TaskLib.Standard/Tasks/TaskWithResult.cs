namespace Rosalia.TaskLib.Standard.Tasks
{
    using Rosalia.Core.Context;
    using Rosalia.Core.Fluent;
    using Rosalia.Core.Tasks;

    public abstract class TaskWithResult<TContext, TResult> : AbstractLeafTask<TContext>
    {
        public TResult Result { get; private set; }

        protected override void Execute(ResultBuilder resultBuilder, TaskContext<TContext> context)
        {
            Result = Execute(context, resultBuilder);
        }

        protected abstract TResult Execute(TaskContext<TContext> context, ResultBuilder resultBuilder);
    }
}