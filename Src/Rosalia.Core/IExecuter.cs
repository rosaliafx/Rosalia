namespace Rosalia.Core
{
    using Rosalia.Core.Result;

    public interface IExecuter<T>
    {
        ExecutionResult Execute(IExecutable<T> task);
    }
}