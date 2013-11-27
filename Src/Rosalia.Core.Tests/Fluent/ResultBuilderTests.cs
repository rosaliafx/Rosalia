namespace Rosalia.Core.Tests.Fluent
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using Rosalia.Core.Fluent;
    using Rosalia.Core.Logging;

    [TestFixture]
    public class ResultBuilderTests
    {
        private static Action<Message> NoOpMessageProcessor
        {
            get { return message => { }; }
        }

        [Test]
        [ExpectedException(ExpectedMessage = "Result type was not set")]
        public void ToExecutionResult_NoResultSet_ShouldThrowException()
        {
            var builder = new ResultBuilder(NoOpMessageProcessor);
            var result = (ExecutionResult)builder;
        }

        [Test]
        public void HasResult_NoResultSet_ReturnsFalse()
        {
            Assert.That(new ResultBuilder(NoOpMessageProcessor).HasResult, Is.False);
        }

        [Test]
        public void HasResult_Succeded_ReturnsTrue()
        {
            Assert.That(new ResultBuilder(NoOpMessageProcessor).Succeed().HasResult, Is.True);
        }

        [Test]
        public void HasResult_Failed_ReturnsTrue()
        {
            Assert.That(new ResultBuilder(NoOpMessageProcessor).Fail().HasResult, Is.True);
        }

        [Test]
        public void AddMessage_ShouldPassToListener()
        {
            var messages = new List<Message>();
            var builder = new ResultBuilder(messages.Add);

            builder.AddMessage(MessageLevel.Info, "{0}", "test");

            Assert.That(messages.Count, Is.EqualTo(1));
        }

        [Test]
        public void AddMessage_TextWithSpecialChars_ShouldNotFail()
        {
            var messages = new List<Message>();
            var builder = new ResultBuilder(messages.Add);

            builder.AddMessage(MessageLevel.Info, "{0}");

            Assert.That(messages.Count, Is.EqualTo(1));
        }
    }
}