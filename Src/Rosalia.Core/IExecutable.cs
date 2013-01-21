namespace Rosalia.Core
{
    using Rosalia.Core.Result;

    public interface IExecutable<TContext>
    {
        ExecutionResult Execute(ExecutionContext<TContext> context);
    }
}