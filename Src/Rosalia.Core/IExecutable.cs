namespace Rosalia.Core
{
    using Rosalia.Core.Context;
    using Rosalia.Core.Result;

    public interface IExecutable<TContext>
    {
        ExecutionResult Execute(TaskContext<TContext> context);
    }
}