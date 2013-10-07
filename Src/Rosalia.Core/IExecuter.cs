namespace Rosalia.Core
{
    public interface IExecuter
    {
        ExecutionResult Execute(IExecutable task);
    }
}