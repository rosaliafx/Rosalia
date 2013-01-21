namespace Rosalia.Core.Result
{
    using System.Collections.Generic;

    public class ExecutionResult
    {
        private readonly IList<ResultMessage> _messages;

        public ExecutionResult(ResultType resultType, IList<ResultMessage> messages)
        {
            ResultType = resultType;
            _messages = messages;
        }

        public ResultType ResultType { get; private set; }

        public IEnumerable<ResultMessage> Messages
        {
            get { return _messages; }
        }
    }
}