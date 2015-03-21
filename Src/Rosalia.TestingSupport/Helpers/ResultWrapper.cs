namespace Rosalia.TestingSupport.Helpers
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using NUnit.Framework.Constraints;
    using Rosalia.Core;
    using Rosalia.Core.Logging;
    using Rosalia.Core.Tasks.Results;

    public class ResultWrapper<T>
    {
        private readonly ITaskResult<T> _result;
        private readonly IList<Tuple<Message, Identity>> _messages;

        public T Data
        {
            get { return _result.Data; }
        }

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

        [Obsolete("todo: use SpuLogRendererMethods")]
        public void AssertHasMessage(MessageLevel level, string message)
        {
            foreach (var messageTuple in _messages)
            {
                if (messageTuple.Item1.Level == level && messageTuple.Item1.Text == message)
                {
                    return;
                }
            }

            Assert.Fail("Message [{0}] with level {1} not found", message, level);
        }

        public void AssertFailure()
        {
            if (_result.IsSuccess)
            {
                Assert.Fail("Expected failure result but was success");
            }
        }

        public void AssertFailedWith(string message)
        {
            AssertFailure();

            Assert.That(_result, Is.InstanceOf<FailureResult<T>>());

            var exception = ((FailureResult<T>) _result).Error;

            Assert.That(exception, Is.Not.Null, "Failure result exception");
            Assert.That(exception.Message, Is.EqualTo(message));
        }
    }
}