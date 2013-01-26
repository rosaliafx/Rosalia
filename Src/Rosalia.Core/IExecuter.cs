namespace Rosalia.Core
{
    public interface IExecuter<T>
    {
        ExecutionResult Execute(IExecutable<T> task);
    }
}