namespace Rosalia.Core.Tasks.Results
{
    public interface ITaskResult<out T>
    {
        bool IsSuccess { get; }

        T Data { get; }

        IError Error { get; }
    }
}