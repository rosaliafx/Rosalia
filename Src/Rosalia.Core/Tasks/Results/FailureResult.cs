namespace Rosalia.Core.Tasks.Results
{
    using System;
    using System.Diagnostics;

    [DebuggerDisplay("{Error.Message}")]
    public class FailureResult<T> : ITaskResult<T>
    {
        private readonly IError _reason;

        public FailureResult(Exception reason) : this(new ExceptionError(reason))
        {
        }

        public FailureResult(string reason) : this(new ConstError(reason))
        {
        }

        public FailureResult(IError reason)
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

        public IError Error 
        {
            get { return _reason; }
        }
    }

    public interface IError
    {
        string Message { get; }
    }

    internal class ConstError : IError
    {
        public ConstError(string message)
        {
            Message = message;
        }

        public string Message { get; private set; }
    }

    internal class ExceptionError : IError
    {
        private readonly Exception _exception;

        public ExceptionError(Exception exception)
        {
            _exception = exception;
        }

        public string Message
        {
            get { return _exception.Message; }
        }
    }
}