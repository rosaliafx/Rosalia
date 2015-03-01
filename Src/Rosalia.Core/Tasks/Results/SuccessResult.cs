namespace Rosalia.Core.Tasks.Results
{
    using System;
    using System.Diagnostics;

    [DebuggerDisplay("{Data}")]
    public class SuccessResult<T>: ITaskResult<T>
    {
        private readonly T _data;

        public SuccessResult(T data)
        {
            _data = data;
        }

        public bool IsSuccess
        {
            get { return true; }
        }

        public T Data
        {
            get { return _data; }
        }

        public Exception Error
        {
            get { return null; }
        }
    }
}