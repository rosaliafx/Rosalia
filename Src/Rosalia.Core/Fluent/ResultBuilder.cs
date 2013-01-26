namespace Rosalia.Core.Fluent
{
    using System;
    using Rosalia.Core.Logging;

    public class ResultBuilder
    {
        private readonly Action<Message> _messageProcessor;
        private ResultType _resultType = ResultType.Success;

        public ResultBuilder(Action<Message> messageProcessor)
        {
            _messageProcessor = messageProcessor;
        }

        public bool IsSuccess
        {
            get { return _resultType == ResultType.Success; }
        }

        private ResultType ResultType
        {
            get { return _resultType; }
        }

        public static implicit operator ExecutionResult(ResultBuilder builder)
        {
            return new ExecutionResult(builder.ResultType);
        }

        public ResultBuilder AddMessage(MessageLevel level, string text, params object[] args)
        {
            var formattedText = string.Format(text, args);
            _messageProcessor.Invoke(new Message(formattedText, level));
            return this;
        }

        public void AddInfo(string mesage, params object[] args)
        {
            AddMessage(MessageLevel.Info, mesage, args);
        }

        public ResultBuilder AddWarning(string text, params object[] args)
        {
            AddMessage(MessageLevel.Warning, text, args);
            return this;
        }

        public ResultBuilder AddError(string text, params object[] args)
        {
            AddMessage(MessageLevel.Error, text, args);
            return this;
        }

        public ResultBuilder Fail()
        {
            _resultType = ResultType.Failure;
            return this;
        }

        public ResultBuilder FailWithError(string text, params object[] args)
        {
            AddError(text, args);
            _resultType = ResultType.Failure;
            return this;
        }

        public ResultBuilder FailWithError(Exception exception)
        {
            AddError(exception.Message);
            _resultType = ResultType.Failure;
            return this;
        }

        public ResultBuilder Succeed()
        {
            _resultType = ResultType.Success;
            return this;
        }
    }
}