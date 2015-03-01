namespace Rosalia.Core.Tasks.Results
{
    public static class ResultsExtensions
    {
        public static ITaskResult<T> AsTaskResult<T>(this T value)
        {
            return new SuccessResult<T>(value);
        }
    }
}