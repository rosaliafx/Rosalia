namespace Rosalia.Core
{
    using Rosalia.Core.Result;

    public interface IGenericWorkflow<in T> : IWorkflow
    {
        ExecutionResult Execute(T inputData);
    }
}