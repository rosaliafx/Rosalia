namespace Rosalia.TaskLib.Standard.Tests
{
    using NUnit.Framework;
    using Rosalia.Core;
    using Rosalia.Core.Tests;

    public class SimpleExternalToolTaskTests : TaskTestsBase<SimpleExternalToolTaskTests.MessageContext>
    {
        [Test]
        public void Execute_EchoTask_ShouldSucceed()
        {
            var echoTask = new SimpleExternalToolTask<MessageContext>(
                c => new SimpleExternalToolTask<MessageContext>.Input("cmd.exe", string.Format("/c echo {0}", c.Data.Message)));

            Data.Message = "Hello, Rosalia!";

            var result = Execute(echoTask);

            Assert.That(result.ResultType, Is.EqualTo(ResultType.Success));
        }

        [Test]
        public void Execute_EchoTask_ShouldLogMessage()
        {
            var echoTask = new SimpleExternalToolTask<MessageContext>(
                c => new SimpleExternalToolTask<MessageContext>.Input("cmd.exe", string.Format("/c echo {0}", c.Data.Message)));

            Data.Message = "Hello, Rosalia!";

            Execute(echoTask);

            Logger.AssertHasInfo();
            Assert.That(Logger.LastInfo.Text, Is.EqualTo("Hello, Rosalia!"));
        }

        [Test]
        public void Execute_EchoTask_ShouldLogSingleMessage()
        {
            var echoTask = new SimpleExternalToolTask<MessageContext>(
                c => new SimpleExternalToolTask<MessageContext>.Input("cmd.exe", string.Format("/c echo {0}", c.Data.Message)));

            Data.Message = "Hello, Rosalia!";

            Execute(echoTask);

            Logger.AssertHasInfo();
            Assert.That(Logger.LastInfo.Text, Is.EqualTo("Hello, Rosalia!"));
        }

        public class MessageContext
        {
            public string Message { get; set; }
        }
    }
}