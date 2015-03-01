namespace Rosalia.Core.Tasks.Results
{
    using System;
    using System.Diagnostics;

    [DebuggerDisplay("{Error.Message}")]
    public class FailureResult<T> : ITaskResult<T>
    {
        private readonly Exception _reason;

        public FailureResult(Exception reason)
        {
            _reason = reason;
        }

        public bool IsSuccess
        {
            get { return false; }
        }

        public T Data
        {
            get { return default(T); }
        }

        public Exception Error 
        {
            get { return _reason; }
        }
    }
}