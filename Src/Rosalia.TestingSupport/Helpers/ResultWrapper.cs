namespace Rosalia.TestingSupport.Helpers
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using Rosalia.Core;
    using Rosalia.Core.Logging;
    using Rosalia.Core.Tasks.Results;

    public class ResultWrapper<T>
    {
        private readonly ITaskResult<T> _result;
        private readonly IList<Tuple<Message, Identity>> _messages;

        public ResultWrapper(ITaskResult<T> result, IList<Tuple<Message, Identity>> messages)
        {
            _result = result;
            _messages = messages;
        }

        public ITaskResult<T> Result
        {
            get { return _result; }
        }

        public IList<Tuple<Message, Identity>> Messages
        {
            get { return _messages; }
        }

        public ResultWrapper<T> AssertSuccess()
        {
            if (!_result.IsSuccess)
            {
                Assert.Fail("Expected success result but was error {0}", _result.Error);
            }

            return this;
        }

        public ResultWrapper<T> AssertDataIs(T expected)
        {
            AssertSuccess();
            Assert.That(_result.Data, Is.EqualTo(expected));

            return this;
        }

        public void AssertHasMessage(MessageLevel level, string message)
        {
            foreach (var messageTuple in _messages)
            {
                if (messageTuple.Item1.Level == level && messageTuple.Item1.Text == message)
                {
                    return;
                }
            }

            Assert.Fail(string.Format("Message [{0}] with level {1} not found", message, level));
        }
    }
}