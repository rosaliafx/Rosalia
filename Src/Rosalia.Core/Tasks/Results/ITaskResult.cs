namespace Rosalia.Core.Tasks.Results
{
    using System;

    public interface ITaskResult<out T>
    {
        bool IsSuccess { get; }

        T Data { get; }

        Exception Error { get; }
    }
}