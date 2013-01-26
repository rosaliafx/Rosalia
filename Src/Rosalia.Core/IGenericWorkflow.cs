namespace Rosalia.Core
{
    public interface IGenericWorkflow<in T> : IWorkflow
    {
        ExecutionResult Execute(T inputData);
    }
}