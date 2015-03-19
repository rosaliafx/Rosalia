namespace Rosalia.Core.Api
{
    using Rosalia.Core.Tasks.Results;

    public static partial class Extensions
    {
        public static ITaskResult<T> AsTaskResult<T>(this T value)
        {
            return new SuccessResult<T>(value);
        }
    }
}