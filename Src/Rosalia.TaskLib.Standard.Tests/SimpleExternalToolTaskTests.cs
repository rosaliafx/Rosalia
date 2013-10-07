namespace Rosalia.TaskLib.Standard.Tests
{
    using NUnit.Framework;
    using Rosalia.Core;
    using Rosalia.Core.Tests;
    using Rosalia.TaskLib.Standard.Tasks;

    public class SimpleExternalToolTaskTests : TaskTestsBase
    {
        [Test]
        public void Execute_EchoTask_ShouldSucceed()
        {
            var echoTask = new SimpleExternalToolTask
            {
                ToolPath = "cmd.exe",
                Arguments = string.Format("/c echo {0}", "Hello, Rosalia!")
            };

            var result = Execute(echoTask);

            Assert.That(result.ResultType, Is.EqualTo(ResultType.Success));
        }

        [Test]
        public void Execute_EchoTask_ShouldLogMessage()
        {
            var echoTask = new SimpleExternalToolTask
            {
                ToolPath = "cmd.exe",
                Arguments = string.Format("/c echo {0}", "Hello, Rosalia!")
            };

            Execute(echoTask);

            Logger.AssertHasInfo();
            Assert.That(Logger.LastInfo.Text, Is.EqualTo("Hello, Rosalia!"));
        }

        [Test]
        public void Execute_EchoTask_ShouldLogSingleMessage()
        {
            var echoTask = new SimpleExternalToolTask
            {
                ToolPath = "cmd.exe",
                Arguments = string.Format("/c echo {0}", "Hello, Rosalia!")
            };
                               
            Execute(echoTask);

            Logger.AssertHasInfo();
            Assert.That(Logger.LastInfo.Text, Is.EqualTo("Hello, Rosalia!"));
        }
    }
}