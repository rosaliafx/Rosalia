namespace Rosalia.Core
{
    public class ExecutionResult
    {
        public ExecutionResult(ResultType resultType)
        {
            ResultType = resultType;
        }

        public ResultType ResultType { get; private set; }
    }
}