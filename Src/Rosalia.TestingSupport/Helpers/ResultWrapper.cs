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
        //private readonly IList<Tuple<Message, Identities>> _messages;
        private readonly SpyLogRenderer _log;

        public T Data
        {
            get { return _result.Data; }
        }

        public ResultWrapper(ITaskResult<T> result, /*IList<Tuple<Message, Identities>> messages,*/ SpyLogRenderer log)
        {
            _result = result;
            _log = log;
            //_messages = messages;
        }

        public ITaskResult<T> Result
        {
            get { return _result; }
        }

        public IList<Tuple<Message, Identities>> Messages
        {
            get { return Log.Messages; }
        }

        public SpyLogRenderer Log
        {
            get { return _log; }
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