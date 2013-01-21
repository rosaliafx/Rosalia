namespace Rosalia.TaskLib.Standard.Tests
{
    using Moq;
    using NUnit.Framework;
    using Rosalia.Core;
    using Rosalia.Core.Logging;
    using Rosalia.Core.Tests;

    public class SimpleExternalToolTaskTests : TaskTestsBase<SimpleExternalToolTaskTests.MessageContext>
    {
        [Test]
        public void Execute_EchoTask_ShouldSucceed()
        {
            var echoTask = new SimpleExternalToolTask<MessageContext>(
                c => new SimpleExternalToolTask<MessageContext>.Input("cmd.exe", string.Format("/c echo {0}", c.Data.Message)));

            var context = CreateContext();
            context.Data.Message = "Hello, Rosalia!";

            var result = echoTask.Execute(context);

            Assert.That(result.ResultType, Is.EqualTo(ResultType.Success));
        }

        [Test]
        public void Execute_EchoTask_ShouldLogMessage()
        {
            var echoTask = new SimpleExternalToolTask<MessageContext>(
                c => new SimpleExternalToolTask<MessageContext>.Input("cmd.exe", string.Format("/c echo {0}", c.Data.Message)));

            var context = CreateContext();
            context.Data.Message = "Hello, Rosalia!";

            echoTask.Execute(context);

            Assert.That(Logger.HasMessage((level, message, args) => level == MessageLevel.Info && message == "Hello, Rosalia!"));
        }

        [Test]
        public void Execute_EchoTask_ShouldLogSingleMessage()
        {
            var echoTask = new SimpleExternalToolTask<MessageContext>(
                c => new SimpleExternalToolTask<MessageContext>.Input("cmd.exe", string.Format("/c echo {0}", c.Data.Message)));

            var context = CreateContext();
            context.Data.Message = "Hello, Rosalia!";

            echoTask.Execute(context);

            Assert.That(Logger.HasMessage((level, message, args) => message == "Hello, Rosalia!"));
        }

        public class MessageContext
        {
            public string Message { get; set; }
        }
    }
}